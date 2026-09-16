using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using WebcamApp.Services;

namespace WebcamApp.Tests;

public class HistogramServiceTests
{
    [Fact]
    public void CalculateHistogram_ShouldReturn256Bins()
    {
        // Arrange
        var histogramService = new HistogramService();

        using var testImage = new Mat(3, 3, DepthType.Cv8U, 1); // Create a 3x3 grayscale image
        testImage.SetTo(new MCvScalar(128));

        // Act
        float[] histogram = histogramService.CalculateHistogram(testImage);

        // Assert
        Assert.Equal(256, histogram.Length);
    }

    [Fact]
    public void CalculateHistogram_ShouldReturnExpectedHistogram()
    {
        // Arrange
        var histogramService = new HistogramService();

        using var testImage = new Mat(3, 3, DepthType.Cv8U, 1);
        testImage.SetTo(new MCvScalar(128));

        // Act
        float[] histogram = histogramService.CalculateHistogram(testImage);

        // Assert
        Assert.Equal(0.0f, histogram[0]);
        Assert.Equal(1.0f, histogram[128]);
        Assert.Equal(0.0f, histogram[255]);
    }

    [Fact]
    public void CalculateHistogram_ShouldHandleMultiplePixelValues()
    {
        // Arrange
        var histogramService = new HistogramService();

        using var testImage = new Mat(3, 3, DepthType.Cv8U, 1);

        // Set the entire image to 128
        testImage.SetTo(new MCvScalar(128));

        // Set the first row to 64
        using var roi = new Mat(
            testImage,
            new System.Drawing.Rectangle(0, 0, 3, 1));

        roi.SetTo(new MCvScalar(64));

        // Act
        float[] histogram = histogramService.CalculateHistogram(testImage);

        // Assert
        Assert.Equal(0.5f, histogram[64]);
        Assert.Equal(1.0f, histogram[128]);
    }
}