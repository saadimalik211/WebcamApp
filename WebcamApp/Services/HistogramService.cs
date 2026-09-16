using Emgu.CV;
using Emgu.CV.CvEnum;

namespace WebcamApp.Services;

public class HistogramService
{
    //histogram calculation method
    public float[] CalculateHistogram(Mat frame)
    {
        //create a Mat that will hold the grayscale version of the frame
        using var grayscaleFrame = new Mat();

        //if the frame is a color image, convert it to grayscale
        if (frame.NumberOfChannels == 3)
        {
            CvInvoke.CvtColor(frame, grayscaleFrame, ColorConversion.Bgr2Gray);
        }
        else
        {
            //if the frame is already grayscale, just copy it
            frame.CopyTo(grayscaleFrame);
        }

        using var hist = new Mat();
        using var images = new Emgu.CV.Util.VectorOfMat(grayscaleFrame);

        // Calculate the histogram for the grayscale image
        CvInvoke.CalcHist(
            images, //input image
            new[] { 0 }, //channel to analyze (grayscale only 1 channel, 0).
            null, //mask (no mask = process all pixels)
            hist, //output histogram
            new[] { 256 }, //specify number of bins. grayscale has 256 possibilities.
            new[] { 0.0f, 256.0f }, //range of pixel values. grayscale is [0,256)
            false); //do not accumulate values from previous histograms

        // Normalize the histogram to the range [0, 1]
        CvInvoke.Normalize(hist, hist, 0.0, 1.0, NormType.MinMax);

        //creating array of floats to store histogram values
        float[] histogram = new float[256];

        //copy the histogram values from the Mat to the float array
        hist.CopyTo(histogram);

        return histogram;
    }
}