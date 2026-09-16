using Emgu.CV;
using Emgu.CV.CvEnum;
using WebcamApp;

namespace WebcamApp.Services;

public class ImageProcessingService
{
    // Process the image by applying each filter in the pipeline in order
    public void ProcessImage(Mat frame, List<ImageMode> filters, double blurValue, double edgeLowerValue, double edgeUpperValue, double bwThresholdValue)
    {
        foreach (var mode in filters)
        {
            switch (mode)
            {
                case ImageMode.Grayscale:
                    ConvertToGrayscale(frame);
                    break;

                case ImageMode.BW:
                    ConvertToGrayscale(frame);
                    CvInvoke.Threshold(frame, frame, bwThresholdValue, 255, ThresholdType.Binary);
                    break;

                case ImageMode.Blur:
                    CvInvoke.GaussianBlur(frame, frame, new System.Drawing.Size((int)blurValue, (int)blurValue), 0, 0);
                    break;

                case ImageMode.EdgeDetection:
                    ConvertToGrayscale(frame);
                    CvInvoke.Canny(frame, frame, edgeLowerValue, edgeUpperValue);
                    break;

                case ImageMode.Invert:
                    CvInvoke.BitwiseNot(frame, frame);
                    break;
            }
        }
    }

    // Convert the frame to grayscale only if it is currently a color image
    private void ConvertToGrayscale(Mat frame)
    {
        if (frame.NumberOfChannels == 3)
        {
            CvInvoke.CvtColor(frame, frame, ColorConversion.Bgr2Gray);
        }
    }
}