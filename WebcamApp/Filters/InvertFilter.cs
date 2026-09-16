using Emgu.CV;

namespace WebcamApp.Filters;

public class InvertFilter : IImageProcessingFilter
{
    public string GetDisplayName()
    {
        return "Invert";
    }

    public void ProcessImage(Mat frame)
    {
        CvInvoke.BitwiseNot(frame, frame);
    }
}