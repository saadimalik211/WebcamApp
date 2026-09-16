using Emgu.CV;
using WebcamApp.Filters;

namespace WebcamApp.Services;

public class ImageProcessingService
{
    // Process the image by applying each filter in the pipeline in order
    public void ProcessImage(Mat frame, List<IImageProcessingFilter> filters)
    {
        foreach (var filter in filters)
        {
            filter.ProcessImage(frame);
        }
    }
}