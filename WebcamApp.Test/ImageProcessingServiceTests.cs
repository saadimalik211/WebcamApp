using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using WebcamApp.Services;

namespace WebcamApp.Tests;

public class ImageProcessingServiceTests
{
    [Fact]
    public void ProcessImage_Grayscale_ShouldConvertToGrayscale()
    {
        // Arrange
        var imageProcessingService = new ImageProcessingService();

        using var testImage = new Mat(1, 1, DepthType.Cv8U, 3);
        testImage.SetTo(new MCvScalar(100, 150, 200));

        var filters = new List<ImageMode>
        {
            ImageMode.Grayscale
        };

        // Act
        imageProcessingService.ProcessImage(
            testImage,
            filters,
            3,
            50,
            150,
            128);

        // Assert
        Assert.Equal(1, testImage.NumberOfChannels);
    }

    [Fact]
    public void ProcessImage_BW_ShouldApplyThreshold()
    {
        // Arrange
        var imageProcessingService = new ImageProcessingService();

        using var testImage = new Mat(1, 2, DepthType.Cv8U, 3);
        testImage.SetTo(new MCvScalar(100, 100, 100));

        // Change the second pixel to 200
        using var roi = new Mat(
            testImage,
            new System.Drawing.Rectangle(1, 0, 1, 1));

        roi.SetTo(new MCvScalar(200, 200, 200));

        var filters = new List<ImageMode>
        {
            ImageMode.BW
        };

        // Act
        imageProcessingService.ProcessImage(
            testImage,
            filters,
            3,
            50,
            150,
            128);

        // Assert
        Assert.Equal(1, testImage.NumberOfChannels);

        // Check the pixel values
        byte[] pixelData = new byte[2];
        testImage.CopyTo(pixelData);

        Assert.Equal(0, pixelData[0]);
        Assert.Equal(255, pixelData[1]);
    }

    [Fact]
    public void ProcessImage_MultipleFilters_ShouldApplyFiltersInOrder()
    {
        // Arrange
        var imageProcessingService = new ImageProcessingService();

        using var testImage = new Mat(1, 1, DepthType.Cv8U, 3);
        testImage.SetTo(new MCvScalar(100, 100, 100));

        var filters = new List<ImageMode>
        {
            ImageMode.Grayscale,
            ImageMode.Invert
        };

        // Act
        imageProcessingService.ProcessImage(
            testImage,
            filters,
            3,
            50,
            150,
            128);

        // Assert
        Assert.Equal(1, testImage.NumberOfChannels);//grayscale should have just the 1 channel

        byte[] pixelData = new byte[1];
        testImage.CopyTo(pixelData);

        Assert.Equal(155, pixelData[0]);//invert of 100 is 155 (255 - 100)
    }
}