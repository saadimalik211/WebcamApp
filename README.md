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
* Ordered filter processing
* Separation of webcam capture, image processing, filter implementations, and histogram calculation
* Unit testing of image-processing functionality

---

## Features

### Live Webcam

* Start and stop the webcam from the UI.
* Continuously captures frames while the webcam is running.
* The live feed updates approximately every 33 milliseconds.
* Webcam initialization runs on a background thread to prevent the WPF UI from freezing while the camera opens.

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

---

## Technologies

* C#
* .NET 10
* WPF
* Emgu CV
* xUnit

---

## Development Environment Setup

This is a Windows desktop application (WPF), so development and execution require Windows.

### Prerequisites

| Requirement                       | Notes                                                                                                                              |
| --------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------- |
| **OS**                            | Windows 10/11 (WPF is Windows-only)                                                                                                |
| **.NET 10 SDK**                   | Download from https://dotnet.microsoft.com/download — required to build and run the app and tests                                  |
| **Visual Studio 2026 (18.10.1+)** | Community edition is sufficient. During installation, select the **.NET desktop development** workload (this includes WPF tooling) |
| **Webcam**                        | A physical or virtual webcam accessible to Windows, with an up-to-date driver, is required for the live-capture features           |
| **Git**                           | For cloning the repository                                                                                                         |

### Steps

1. Install the .NET 10 SDK.

2. Install Visual Studio 2026 with the **.NET desktop development** workload checked.

3. Clone the repository:

   ```bash
   git clone <repository-url>
   cd WebcamApp
   ```

4. Confirm the SDK is installed correctly:

   ```bash
   dotnet --version
   ```

   This should report a `10.x` version.

---

## Installing Dependencies

Dependencies are managed via NuGet and are declared in the project files (`.csproj`), so they do not need to be installed manually. Restoring the solution downloads them automatically.

**Key NuGet packages used:**

* `Emgu.CV` and `Emgu.CV.runtime.windows` – webcam capture and image processing (OpenCV wrapper)
* `xunit` and `xunit.runner.visualstudio` – unit testing framework (test project only)

### Option A — Visual Studio

1. Open **File → Open → Project/Solution...** in Visual Studio and select `WebcamApp.slnx` at the repository root. This repository uses the newer `.slnx` solution file format rather than the classic `.sln`.
2. Visual Studio will normally restore NuGet packages automatically when the solution is loaded. If it does not, right-click the solution in **Solution Explorer** and select **Restore NuGet Packages**.

### Option B — .NET CLI

From the repository root:

```bash
dotnet restore
```

This resolves and downloads all dependencies listed in the `.csproj` files for both the main project and the test project.

---

## Build and Run

### Option A — Visual Studio

1. Open **File → Open → Project/Solution...** in Visual Studio and select `WebcamApp.slnx` at the repository root.
2. Restore NuGet packages if they were not restored automatically.
3. Set `WebcamApp` as the **Startup Project** if it is not already.
4. Build the solution using **Build → Build Solution** or `Ctrl+Shift+B`.
5. Ensure a webcam is available to the computer.
6. Run the application using **Debug → Start Debugging** (`F5`) or **Start Without Debugging** (`Ctrl+F5`).
7. Click **Start Webcam**.
8. Add filters to the active pipeline as desired.
9. Adjust filter settings using the sliders.

### Option B — .NET CLI

From the solution root:

```bash
dotnet build
dotnet run --project WebcamApp
```

> **Note:** WPF applications built with the CLI still require Windows to run because WPF depends on Windows-specific UI frameworks.

---

## Running Tests

The unit tests are contained in the `WebcamApp.Tests` project and use xUnit.

### Option A — Visual Studio

Use **Test → Run All Tests** from Test Explorer.

### Option B — .NET CLI

From the solution root:

```bash
dotnet test
```

---

## Project Structure

```text
WebcamApp/
│
├── MainWindow.xaml
├── MainWindow.xaml.cs
│
├── Filters/
│   ├── IImageProcessingFilter.cs
│   ├── GrayscaleFilter.cs
│   ├── BlackWhiteFilter.cs
│   ├── BlurFilter.cs
│   ├── EdgeDetectionFilter.cs
│   └── InvertFilter.cs
│
└── Services/
    ├── WebcamService.cs
    ├── ImageProcessingService.cs
    └── HistogramService.cs

WebcamApp.Tests/
│
├── HistogramServiceTests.cs
└── ImageProcessingServiceTests.cs
```

### MainWindow

Responsible for the WPF user interface and coordinating the application.

This includes:

* Start/stop controls
* Filter selection
* Filter settings
* Coordinating frame processing
* Displaying the webcam image
* Drawing and displaying the histogram
* Running the capture loop

### WebcamService

Responsible for webcam interaction.

It provides methods to:

* Start the webcam
* Capture frames
* Stop and dispose of the webcam

Keeping webcam functionality in its own service separates device interaction from image processing and UI logic.

### ImageProcessingService

Responsible for executing the configured filter pipeline.

The service receives a frame and the active ordered filter list. It processes each filter sequentially using the common `IImageProcessingFilter` interface.

Because the service works with the interface rather than individual concrete filter types, it does not need to know which specific filters are currently in the pipeline.

### Image Processing Filters

Image-processing filters implement the `IImageProcessingFilter` interface.

The interface defines the common operations required by each filter:

* `GetDisplayName()` – provides the filter name displayed in the UI.
* `ProcessImage(Mat frame)` – applies the filter to the current frame.

Concrete implementations include:

* `GrayscaleFilter`
* `BlackWhiteFilter`
* `BlurFilter`
* `EdgeDetectionFilter`
* `InvertFilter`

Each filter owns its specific image-processing behavior and any configurable values it requires.

This allows `ImageProcessingService` to process an ordered list of filters without needing to know the specific filter types in the pipeline.

Filters modify the supplied `Mat` in place rather than returning a new image.

### HistogramService

Responsible for calculating the grayscale histogram.

The service accepts the processed frame. If the frame is still a color image, it creates a grayscale copy specifically for histogram calculation.

Emgu CV's histogram functionality is then used to calculate 256 grayscale intensity bins. The resulting histogram is normalized for display and returned as a `float[256]`.

### WebcamApp.Tests

Contains the xUnit test project covering image-processing and histogram logic in isolation from the WPF UI and physical webcam.

---

## Architecture

The application separates UI presentation and coordination, webcam interaction, image processing, individual filter implementations, and histogram calculation.

```text
                         ┌─────────────────────┐
                         │       Webcam        │
                         └──────────┬──────────┘
                                    │
                                    ▼
                         ┌─────────────────────┐
                         │   WebcamService     │
                         │                     │
                         │  • Start camera     │
                         │  • Capture frames   │
                         │  • Stop camera      │
                         └──────────┬──────────┘
                                    │
                                Mat frame
                                    │
                                    ▼
                         ┌─────────────────────┐
                         │     MainWindow      │
                         │      WPF / UI       │
                         └──────────┬──────────┘
                                    │
                         Mat + Filter Pipeline
                                    │
                                    ▼
                         ┌─────────────────────┐
                         │ ImageProcessing     │
                         │ Service             │
                         └──────────┬──────────┘
                                    │
                                    ▼
                         ┌─────────────────────┐
                         │IImageProcessingFilter│
                         └──────────┬──────────┘
                                    │
                              implementations
                                    │
                 ┌────────┬─────────┼─────────┬────────┐
                 ▼        ▼         ▼         ▼        ▼
             Grayscale   B&W       Blur      Edge    Invert
                 │        │         │         │        │
                 └────────┴─────────┴─────────┴────────┘
                                    │
                          Mat modified in place
                                    │
                                    ▼
                              MainWindow
                               │       │
                 Processed Mat │       │ Processed Mat
                               ▼       ▼
                       Histogram     DisplayFrame
                        Service          │
                           │             ▼
                      float[256]     Webcam Image
                           │
                           ▼
                    DrawHistogram
                           │
                           ▼
                    Histogram Canvas
```

### Component Responsibilities

The main responsibilities can be summarized as:

```text
MainWindow
    │
    ├── Coordinates application flow
    ├── Handles WPF controls
    ├── Displays processed frames
    └── Draws histogram
     
WebcamService
    └── Handles webcam interaction

ImageProcessingService
    └── Executes the ordered filter pipeline

IImageProcessingFilter
    └── Defines the contract used by all image filters

Filter Implementations
    └── Perform individual image-processing operations

HistogramService
    └── Creates and calculates the grayscale histogram
```

---

## Processing Flow

Each captured frame moves through the application using the following general flow:

```text
Capture Frame
     │
     ▼
Apply Active Filters in Pipeline Order
     │
     ▼
Calculate Grayscale Histogram
     │
     ├──────────────► Draw Histogram
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

The same `Mat` is modified in place as it moves through the filter pipeline. After filter processing is complete, `MainWindow` continues using the processed frame for histogram calculation and display.

The histogram calculation does not require the displayed frame itself to be grayscale. If the processed frame is still a color image, `HistogramService` creates a separate grayscale copy for histogram calculation.

---

## Testing

The project contains an xUnit test project covering core image-processing functionality.

The automated tests focus on deterministic image-processing and histogram behavior. Webcam interaction and real-time WPF behavior are tested manually because they depend on physical hardware and UI interaction.

### Automated Tests

The current unit tests cover:

#### Histogram

* Histogram calculation returns 256 bins.
* A uniform grayscale image produces the expected dominant intensity value.
* Histogram behavior with multiple pixel values is tested.
* A color image is converted to grayscale correctly for histogram calculation.

#### Image Processing

* Grayscale conversion changes a color image to a single-channel image.
* Black & white thresholding produces the expected binary pixel values.
* Multiple filters are applied sequentially and in the expected order.
* Invert produces the expected inverted pixel value.
* An empty filter pipeline leaves the image unchanged.

The tests use small programmatically generated `Mat` images with known pixel values. This allows the expected results to be compared directly against the output of the processing and histogram services.

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
* Verifying that the UI remains responsive while the webcam initializes.
* Verifying that the application handles an unavailable webcam without crashing.
* Verifying behavior when the webcam stops providing frames.

The physical webcam and real-time UI are intentionally tested at the application level rather than mocked in the unit tests.

For a larger application, webcam access could additionally be placed behind an interface so that a fake webcam implementation could provide predetermined frames for automated capture and live-view testing.

---

## Assumptions and Design Decisions

### Development Environment

The application targets Windows only because WPF has no cross-platform runtime.

Development assumes Visual Studio 2026 and the .NET 10 SDK are available. The .NET CLI is supported as an alternative for building, running, and testing outside the IDE.

### Service Separation

Webcam interaction, image processing, and histogram calculation are separated into dedicated services.

`MainWindow` remains responsible for WPF-specific UI behavior and coordinating the application's processing flow.

This keeps device interaction and image-processing logic separate from presentation logic without introducing additional architectural complexity beyond the scope of the project.

### Filter Architecture

All image filters implement `IImageProcessingFilter`.

`ImageProcessingService` therefore operates on a `List<IImageProcessingFilter>` rather than containing conditional logic for every available filter type.

Each concrete filter is responsible for its own processing behavior and configurable values.

This design keeps the processing service independent of the individual filter implementations and allows additional filters to be introduced without changing the processing loop.

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

Filters that require grayscale input check the current number of image channels before performing grayscale conversion. This allows filters to be combined without repeatedly attempting to convert an already-grayscale image.

### In-Place Image Processing

Filters operate on the same `Mat` frame and modify it in place.

This keeps the processing pipeline simple because `MainWindow` can pass the captured frame through the filter pipeline and continue using that same frame for histogram calculation and display.

### UI Responsiveness

Opening a webcam through OpenCV can be a synchronous operation and may take several seconds depending on the device and system.

Webcam initialization is therefore performed on a background thread using `Task.Run()` and awaited by the capture loop. This prevents camera initialization from blocking the WPF UI thread.

The capture loop also uses asynchronous delays to control the frame rate while allowing the WPF UI to continue processing user interaction.

The current implementation prioritizes clarity and simplicity appropriate for the scope of this project.

### Resource Management

Objects that hold unmanaged image resources, such as `Mat`, `Bitmap`, and `VideoCapture`, are disposed when they are no longer needed.

The webcam is stopped and released when the capture loop exits, including when the loop is cancelled or an error causes it to stop.

---

## Sample Output

Screenshots of the application running with the webcam feed, filters, and histogram are included with the project.

These demonstrate:

* Live webcam capture
* Real-time histogram generation
* Individual image filters
* Multiple filters applied through the processing pipeline
* Adjustable filter parameters

---

## Summary

The application demonstrates a real-time webcam image-processing pipeline using C#, WPF, and Emgu CV.

The implementation separates webcam capture, image processing, filter implementations, and histogram calculation while keeping the WPF window responsible for coordinating UI and application flow.

A common `IImageProcessingFilter` interface allows multiple concrete filters to participate in the same ordered processing pipeline, while dedicated services keep webcam interaction, image processing, and histogram calculation separated by responsibility.

Automated xUnit tests validate deterministic image-processing and histogram behavior, while physical webcam and real-time UI behavior are verified through application-level testing.
