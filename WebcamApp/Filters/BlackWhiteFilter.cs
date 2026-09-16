using Emgu.CV;
using Emgu.CV.CvEnum;

namespace WebcamApp.Filters;

public class BlackWhiteFilter : IImageProcessingFilter
{
    public double ThresholdValue { get; set; } = 128;

    public string GetDisplayName()
    {
        return "Black & White";
    }

    public void ProcessImage(Mat frame)
    {
        if (frame.NumberOfChannels == 3)
        {
            CvInvoke.CvtColor(frame, frame, ColorConversion.Bgr2Gray);
        }

        CvInvoke.Threshold(frame, frame, ThresholdValue, 255, ThresholdType.Binary);
    }
}