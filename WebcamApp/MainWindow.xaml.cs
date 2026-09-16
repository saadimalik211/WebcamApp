using Emgu.CV;
using WebcamApp.Services;
using WebcamApp.Filters;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WebcamApp;

public partial class MainWindow : Window
{
    // Services
    private readonly WebcamService _webcamService;
    private readonly HistogramService _histogramService;
    private readonly ImageProcessingService _imageProcessingService;

    // Webcam state
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _isRunning = false;

    // Available configurable filters
    private readonly BlurFilter _blurFilter = new();
    private readonly BlackWhiteFilter _blackWhiteFilter = new();
    private readonly EdgeDetectionFilter _edgeDetectionFilter = new();

    // Available filters
    private readonly List<IImageProcessingFilter> _availableFilters = new();

    // Ordered list of filters in the current pipeline
    private readonly List<IImageProcessingFilter> _filterPipeline = new();

    public MainWindow()
    {
        InitializeComponent();

        _webcamService = new WebcamService();
        _histogramService = new HistogramService();
        _imageProcessingService = new ImageProcessingService();

        StartButton.IsEnabled = true;
        StopButton.IsEnabled = false;

        // Create the available filters
        _availableFilters.Add(new GrayscaleFilter());
        _availableFilters.Add(_blackWhiteFilter);
        _availableFilters.Add(_blurFilter);
        _availableFilters.Add(_edgeDetectionFilter);
        _availableFilters.Add(new InvertFilter());

        // Display the filter names in the UI
        foreach (var filter in _availableFilters)
        {
            AvailableFilterList.Items.Add(filter.GetDisplayName());
        }

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
        {
            // Start the webcam
            if (!_webcamService.Start())
            {
                MessageBox.Show("Unable to open the webcam.");

                // If webcam fails to open, reset application state
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
                    MessageBox.Show("The webcam is not providing frames.");
                    break;
                }

                // Process and apply the filters in the pipeline
                _imageProcessingService.ProcessImage(frame, _filterPipeline);

                // Calculate and draw the grayscale histogram
                float[] histogram = _histogramService.CalculateHistogram(frame);
                DrawHistogram(histogram);

                // Display the processed frame
                DisplayFrame(frame);

                // Delay to control the frame rate
                await Task.Delay(33, token);
            }
        }
        catch (OperationCanceledException)
        {
            // Handle cancellation gracefully
        }

        // Clean up when the loop ends
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
            { //filling the canvas with rectangles for histogram bars
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

    // Convert an OpenCV Mat to a WPF-compatible image and display it
    private void DisplayFrame(Mat frame)
    {
        //convert the Mat to a Bitmap
        using var bitmap = frame.ToBitmap();

        //create a memory stream to temporarily hold the bitmap data
        using var stream = new MemoryStream();

        //save the bitmap into the stream
        bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);

        //reset the stream position so it can be read from the beginning
        stream.Position = 0;

        //create a WPF-compatible BitmapSource from the stream
        var bitmapSource = BitmapFrame.Create(
            stream,
            BitmapCreateOptions.None,
            BitmapCacheOption.OnLoad);

        //display the frame in the webcam image control
        WebcamImage.Source = bitmapSource;
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
        int selectedIndex = AvailableFilterList.SelectedIndex;

        if (selectedIndex >= 0)
        {
            IImageProcessingFilter selectedFilter = _availableFilters[selectedIndex];

            _filterPipeline.Add(selectedFilter);
            FilterPipelineList.Items.Add(selectedFilter.GetDisplayName());
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
        _blurFilter.BlurValue = e.NewValue;
    }

    // Edge lower threshold slider
    private void EdgeLowerSlider_ValueChanged(
        object sender,
        RoutedPropertyChangedEventArgs<double> e)
    {
        _edgeDetectionFilter.LowerThreshold = e.NewValue;
    }

    // Edge upper threshold slider
    private void EdgeUpperSlider_ValueChanged(
        object sender,
        RoutedPropertyChangedEventArgs<double> e)
    {
        _edgeDetectionFilter.UpperThreshold = e.NewValue;
    }

    // BW threshold slider
    private void BWThresholdSlider_ValueChanged(
        object sender,
        RoutedPropertyChangedEventArgs<double> e)
    {
        _blackWhiteFilter.ThresholdValue = e.NewValue;
    }
}