# WebcamApp

A C# WPF application that captures a live webcam feed, applies configurable image-processing filters, converts frames to grayscale for histogram analysis, and displays the resulting grayscale histogram in real time.

## Overview

This project was created as a take-home image-processing exercise.

The application continuously captures frames from a webcam and displays the live video feed. Users can add one or more image-processing filters to a pipeline and adjust filter parameters while the webcam is running.

A grayscale histogram is generated continuously from the current frame and displayed alongside the live video.

The main goals of the implementation are:

* Real-time webcam capture
* 8-bit grayscale image processing
* Real-time grayscale histogram generation
* Multiple user-selectable image filters
* Configurable filter parameters
* Separation of webcam capture, image processing, and histogram calculation
* Unit testing of image-processing functionality

## Features

### Live Webcam

* Start and stop the webcam from the UI.
* Continuously captures frames while the webcam is running.
* The live feed updates approximately every 33 milliseconds.

### Grayscale Histogram

A 256-bin histogram is calculated from the grayscale version of the current frame.

The histogram represents the distribution of grayscale intensity values from 0 to 255.

The histogram is updated continuously while the webcam is running.

### Image Filters

The application supports the following filters:

* **Grayscale** – converts the image to a single-channel grayscale image.
* **Black & White (BW)** – converts the image to grayscale and applies a binary threshold.
* **Blur** – applies a Gaussian blur.
* **Edge Detection** – converts the image to grayscale and applies Canny edge detection.
* **Invert** – inverts the pixel values.

Multiple filters can be added to the active pipeline. Filters are applied sequentially in the order they were added.

Filters can be added or removed while the webcam is running without stopping the live feed.

### Filter Controls

The application provides adjustable parameters for:

* Gaussian blur kernel size
* Black & white threshold
* Canny lower threshold
* Canny upper threshold

These values can be changed while the webcam is running.

## Technologies

* C#
* .NET 10
* WPF
* Emgu CV
* xUnit

## Dependencies

The project uses Emgu CV for webcam capture and image processing.

The test project uses xUnit for unit testing.

NuGet dependencies are defined in the project files.

## Project Structure

```text
PracticingWebcam5/
│
├── MainWindow.xaml
├── MainWindow.xaml.cs
│
└── Services/
    ├── WebcamService.cs
    ├── ImageProcessingService.cs
    └── HistogramService.cs
```

### MainWindow

Responsible for the WPF user interface and coordinating the application.

This includes:

* Start/stop controls
* Filter selection
* Filter settings
* Displaying the webcam image
* Displaying the histogram
* Running the capture loop

### WebcamService

Responsible for webcam interaction.

It provides methods to:

* Start the webcam
* Capture frames
* Stop and dispose of the webcam

Keeping webcam functionality in its own service separates device interaction from image processing and UI logic.

### ImageProcessingService

Responsible for applying the configured filter pipeline.

The service receives a frame and the active filter list, then processes each filter sequentially.

Filters that require grayscale conversion perform the conversion when necessary.

### HistogramService

Responsible for calculating the grayscale histogram.

The service uses Emgu CV's histogram functionality to calculate 256 grayscale bins and normalizes the resulting values for display.

## Processing Pipeline

The general processing flow is:

```text
Webcam
   │
   ▼
Capture Frame
   │
   ▼
Apply Active Filters
   │
   ▼
Create Grayscale Histogram Input
   │
   ▼
Calculate Histogram
   │
   ├──────────────► Display Histogram
   │
   ▼
Display Processed Frame
```

The active filters are processed sequentially.

For example:

```text
Grayscale → Blur → Invert
```

will first convert the frame to grayscale, then blur it, and finally invert the resulting image.

## Testing

The project contains an xUnit test project covering core image-processing functionality.

### Automated Tests

The current unit tests cover:

#### Histogram

* Histogram calculation returns 256 bins.
* A uniform grayscale image produces the expected dominant intensity value.
* Histogram behavior with multiple pixel values is tested.

#### Image Processing

* Grayscale conversion changes a color image to a single-channel image.
* Black & white thresholding produces the expected binary pixel values.
* Multiple filters are applied sequentially and in the expected order.

### Manual Testing

Some functionality depends on the physical webcam and WPF UI, so it is tested by running the application.

Manual testing includes:

* Starting the webcam and verifying that frames are displayed.
* Stopping the webcam and verifying that the live feed stops.
* Verifying that the histogram updates continuously while the webcam is running.
* Adding and removing filters while the webcam is running.
* Verifying that multiple filters are applied in pipeline order.
* Adjusting blur, BW threshold, and edge-detection settings while the live feed is running.
* Starting and stopping the webcam repeatedly to verify normal application behavior.

The physical webcam and real-time UI are intentionally tested at the application level rather than mocked in the unit tests.

## Assumptions and Design Decisions

### Grayscale Histogram

The histogram represents grayscale intensity values from 0–255.

If the processed frame is still a color image, a grayscale copy is created specifically for histogram calculation. This allows the histogram to remain available even when the displayed image is still in color.

### Filter Ordering

Filters are stored in an ordered list and processed sequentially.

This allows combinations such as:

```text
Grayscale → Blur → Invert
```

or:

```text
Blur → Grayscale → Edge Detection
```

The processing service checks the current number of image channels before performing grayscale conversion so that filters can be combined without repeatedly attempting to convert an already-grayscale image.

### UI Responsiveness

The capture loop uses asynchronous delays to control the frame rate and allow the WPF UI to continue processing user interaction.

The current implementation prioritizes clarity and simplicity appropriate for the scope of this project.

## Running the Application

1. Open the solution in Visual Studio.
2. Restore NuGet packages.
3. Build the solution.
4. Ensure a webcam is available to the computer.
5. Run the `PracticingWebcam5` project.
6. Click **Start Webcam**.
7. Add filters to the active pipeline as desired.
8. Adjust filter settings using the sliders.

## Running Tests

The unit tests are contained in the `PracticingWebcam5.Tests` project.

Tests can be run from Visual Studio using:

**Test → Run All Tests**

The test project can also be executed using the .NET CLI:

```bash
dotnet test
```

## Sample Output

Screenshots of the application running with the webcam feed, filters, and histogram are included with the project.

These demonstrate the application's live-view functionality and image-processing output.

## Future Improvements

Possible improvements beyond the current implementation include:

* More robust handling of unavailable or disconnected webcams
* Additional image-processing filters
* Improved performance by moving more image processing away from the UI thread
* A more formal MVVM architecture
* Additional integration tests
* An architecture diagram

## Summary

The application demonstrates a real-time webcam processing pipeline using C#, WPF, and Emgu CV.

The implementation separates webcam capture, image processing, and histogram calculation into dedicated services while keeping the WPF window responsible for coordinating the UI and application flow.
