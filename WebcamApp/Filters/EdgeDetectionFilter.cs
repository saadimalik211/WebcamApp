using Emgu.CV;
using Emgu.CV.CvEnum;

namespace WebcamApp.Filters;

public class EdgeDetectionFilter : IImageProcessingFilter
{
    public double LowerThreshold { get; set; } = 50;
    public double UpperThreshold { get; set; } = 150;

    public string GetDisplayName()
    {
        return "Edge Detection";
    }

    public void ProcessImage(Mat frame)
    {
        if (frame.NumberOfChannels == 3)
        {
            CvInvoke.CvtColor(frame, frame, ColorConversion.Bgr2Gray);
        }

        CvInvoke.Canny(frame, frame, LowerThreshold, UpperThreshold);
    }
}