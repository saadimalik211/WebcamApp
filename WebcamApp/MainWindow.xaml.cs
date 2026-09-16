using Emgu.CV;
using Emgu.CV.CvEnum;
using WebcamApp.Services;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace WebcamApp;

// Enum to represent the different image filters
public enum ImageMode
{
    Grayscale,
    BW,
    Blur,
    EdgeDetection,
    Invert
}

public partial class MainWindow : Window
{
    // Services
    private readonly WebcamService _webcamService;
    private readonly HistogramService _histogramService;
    private readonly ImageProcessingService _imageProcessingService;

    // Webcam state
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _isRunning = false;

    // Filter settings
    private double _blurValue = 15;
    private double _edgeLowerValue = 50;
    private double _edgeUpperValue = 150;
    private double _bwThresholdValue = 128;

    // Ordered list of filters in the current pipeline
    private readonly List<ImageMode> _filterPipeline = new();

    public MainWindow()
    {
        InitializeComponent();

        _webcamService = new WebcamService();
        _histogramService = new HistogramService();
        _imageProcessingService = new ImageProcessingService();

        StartButton.IsEnabled = true;
        StopButton.IsEnabled = false;

        // Add the available filters
        AvailableFilterList.Items.Add(ImageMode.Grayscale);
        AvailableFilterList.Items.Add(ImageMode.BW);
        AvailableFilterList.Items.Add(ImageMode.Blur);
        AvailableFilterList.Items.Add(ImageMode.EdgeDetection);
        AvailableFilterList.Items.Add(ImageMode.Invert);

        // Subscribe to the Closing event of the window
        this.Closing += MainWindow_Closing;
    }

    // Handle the Closing event to cancel the capture loop
    private void MainWindow_Closing(object? sender, CancelEventArgs e)
    {
        _cancellationTokenSource?.Cancel();
    }

    // CaptureLoop continuously captures frames from the webcam
    private async Task CaptureLoop(CancellationToken token)
    {
        try
        {   // Start the webcam
            if (!_webcamService.Start())
            {
                MessageBox.Show("Unable to open the webcam.");
                //if webcam fails to open, make sure we set the right state on buttons and isrunning flag.
                _isRunning = false;
                StartButton.IsEnabled = true;
                StopButton.IsEnabled = false;

                return;
            }
            while (!token.IsCancellationRequested)
            {
                using var frame = _webcamService.CaptureFrame();

                if (frame == null)
                {
                    MessageBox.Show("The webcam stopped providing frames.");
                    break;
                }

                // Process and apply the filters in the pipeline
                _imageProcessingService.ProcessImage(
                    frame,
                    _filterPipeline,
                    _blurValue,
                    _edgeLowerValue,
                    _edgeUpperValue,
                    _bwThresholdValue);

                // Create a grayscale copy for histogram calculation
                using var histogramFrame = new Mat();

                if (frame.NumberOfChannels == 3)
                {
                    CvInvoke.CvtColor(frame, histogramFrame, ColorConversion.Bgr2Gray);
                }
                else
                {
                    frame.CopyTo(histogramFrame);
                }

                // Calculate and draw the histogram
                float[] histogram = _histogramService.CalculateHistogram(histogramFrame);
                DrawHistogram(histogram);

                // Convert the processed frame to a WPF-compatible image
                using var bitmap = frame.ToBitmap();
                using var stream = new MemoryStream();

                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                stream.Position = 0;
                var bitmapSource = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

                //finally, we set the source of webcamimage and this updates the UI.
                WebcamImage.Source = bitmapSource;

                // Delay to control the frame rate
                await Task.Delay(33, token);
            }
        }
        catch (OperationCanceledException)
        {
            // Handle cancellation gracefully
        }

        // Clean up when the loop is cancelled
        _webcamService.Stop();

        _isRunning = false;
        StartButton.IsEnabled = true;
        StopButton.IsEnabled = false;

        WebcamImage.Source = null;
        HistogramCanvas.Children.Clear();
    }

    private void DrawHistogram(float[] histogram)
    {
        // Clear the existing histogram bars
        HistogramCanvas.Children.Clear();

        for (int i = 0; i < 256; i++)
        {
            var bar = new System.Windows.Shapes.Rectangle
            {
                Width = HistogramCanvas.ActualWidth / 256,
                Height = histogram[i] * HistogramCanvas.ActualHeight,
                Fill = System.Windows.Media.Brushes.Black
            };

            // Put each bar next to the previous one
            Canvas.SetLeft(bar, i * bar.Width);

            // Set the top position of the bar
            Canvas.SetTop(
                bar,
                HistogramCanvas.ActualHeight - bar.Height);

            // Add the bar to the histogram canvas
            HistogramCanvas.Children.Add(bar);
        }
    }

    // Start the webcam
    private async void StartWebcam_Click(object sender, RoutedEventArgs e)
    {
        if (_isRunning)
            return;

        _isRunning = true;

        StartButton.IsEnabled = false;
        StopButton.IsEnabled = true;

        _cancellationTokenSource = new CancellationTokenSource();

        await CaptureLoop(_cancellationTokenSource.Token);
    }

    // Stop the webcam
    private void StopWebcam_Click(object sender, RoutedEventArgs e)
    {
        if (_isRunning)
        {
            _cancellationTokenSource?.Cancel();
        }
    }

    // Add the selected filter to the pipeline
    private void AddFilter_Click(object sender, RoutedEventArgs e)
    {
        if (AvailableFilterList.SelectedItem is ImageMode selectedFilter)
        {
            _filterPipeline.Add(selectedFilter);
            FilterPipelineList.Items.Add(selectedFilter);
        }
    }

    // Remove the selected filter from the pipeline
    private void RemoveFilter_Click(object sender, RoutedEventArgs e)
    {
        if (FilterPipelineList.SelectedIndex >= 0)
        {
            int selectedIndex = FilterPipelineList.SelectedIndex;

            _filterPipeline.RemoveAt(selectedIndex);
            FilterPipelineList.Items.RemoveAt(selectedIndex);
        }
    }

    // Blur slider
    private void BlurSlider_ValueChanged(
        object sender,
        RoutedPropertyChangedEventArgs<double> e)
    {
        _blurValue = e.NewValue;
    }

    // Edge lower threshold slider
    private void EdgeLowerSlider_ValueChanged(
        object sender,
        RoutedPropertyChangedEventArgs<double> e)
    {
        _edgeLowerValue = e.NewValue;
    }

    // Edge upper threshold slider
    private void EdgeUpperSlider_ValueChanged(
        object sender,
        RoutedPropertyChangedEventArgs<double> e)
    {
        _edgeUpperValue = e.NewValue;
    }

    // BW threshold slider
    private void BWThresholdSlider_ValueChanged(
        object sender,
        RoutedPropertyChangedEventArgs<double> e)
    {
        _bwThresholdValue = e.NewValue;
    }
}