using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using WebcamApp.Filters;
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

        var filters = new List<IImageProcessingFilter>
        {
            new GrayscaleFilter()
        };

        // Act
        imageProcessingService.ProcessImage(testImage, filters);

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

        var blackWhiteFilter = new BlackWhiteFilter
        {
            ThresholdValue = 128
        };

        var filters = new List<IImageProcessingFilter>
        {
            blackWhiteFilter
        };

        // Act
        imageProcessingService.ProcessImage(testImage, filters);

        // Assert
        Assert.Equal(1, testImage.NumberOfChannels);

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

        var filters = new List<IImageProcessingFilter>
        {
            new GrayscaleFilter(),
            new InvertFilter()
        };

        // Act
        imageProcessingService.ProcessImage(testImage, filters);

        // Assert
        // Grayscale should result in one channel
        Assert.Equal(1, testImage.NumberOfChannels);

        byte[] pixelData = new byte[1];
        testImage.CopyTo(pixelData);

        // Invert of 100 is 155 (255 - 100)
        Assert.Equal(155, pixelData[0]);
    }

    [Fact]
    public void ProcessImage_Invert_ShouldInvertPixelValue()
    {
        // Arrange
        var imageProcessingService = new ImageProcessingService();

        using var testImage = new Mat(1, 1, DepthType.Cv8U, 1);
        testImage.SetTo(new MCvScalar(100));

        var filters = new List<IImageProcessingFilter>
        {
            new InvertFilter()
        };

        // Act
        imageProcessingService.ProcessImage(testImage, filters);

        // Assert
        byte[] pixelData = new byte[1];
        testImage.CopyTo(pixelData);

        Assert.Equal(155, pixelData[0]);
    }

    [Fact]
    public void ProcessImage_NoFilters_ShouldNotModifyImage()
    {
        // Arrange
        var imageProcessingService = new ImageProcessingService();

        using var testImage = new Mat(1, 1, DepthType.Cv8U, 1);
        testImage.SetTo(new MCvScalar(100));

        var filters = new List<IImageProcessingFilter>();

        // Act
        imageProcessingService.ProcessImage(testImage, filters);

        // Assert
        byte[] pixelData = new byte[1];
        testImage.CopyTo(pixelData);

        Assert.Equal(100, pixelData[0]);
    }
}