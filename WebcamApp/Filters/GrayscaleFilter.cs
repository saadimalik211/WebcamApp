using Emgu.CV;
using Emgu.CV.CvEnum;

namespace WebcamApp.Filters;

public class GrayscaleFilter : IImageProcessingFilter
{
    public string GetDisplayName()
    {
        return "Grayscale";
    }

    public void ProcessImage(Mat frame)
    {
        if (frame.NumberOfChannels == 3)
        {
            CvInvoke.CvtColor(frame, frame, ColorConversion.Bgr2Gray);
        }
    }
}