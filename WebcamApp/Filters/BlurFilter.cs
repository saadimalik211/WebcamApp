using Emgu.CV;

namespace WebcamApp.Filters;

public class BlurFilter : IImageProcessingFilter
{
    public double BlurValue { get; set; } = 15;

    public string GetDisplayName()
    {
        return "Blur";
    }

    public void ProcessImage(Mat frame)
    {
        CvInvoke.GaussianBlur(
            frame,
            frame,
            new System.Drawing.Size((int)BlurValue, (int)BlurValue),
            0,
            0);
    }
}