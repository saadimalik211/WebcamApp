using Emgu.CV;

namespace WebcamApp.Filters;

public interface IImageProcessingFilter
{
    string GetDisplayName();

    void ProcessImage(Mat frame);
}