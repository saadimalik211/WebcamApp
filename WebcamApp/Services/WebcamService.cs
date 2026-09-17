using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebcamApp.Services
{
    public class WebcamService
    {
        private VideoCapture? _capture;

        public bool Start()
        {
            try
            {
                _capture = new VideoCapture();

                if (!_capture.IsOpened)
                {
                    _capture.Dispose();
                    _capture = null;
                    return false;
                }

                return true;
            }
            catch
            {
                _capture?.Dispose();
                _capture = null;
                return false;
            }
        }

        public Mat? CaptureFrame()
        {
            var frame = new Mat();

            // Is there a webcam?
            if (_capture == null)
            {
                frame.Dispose();
                return null;
            }

            // Try to read a frame from the webcam
            bool readSuccessful = _capture.Read(frame);

            if (!readSuccessful)
            {
                frame.Dispose();
                return null;
            }

            // Make sure the frame actually contains image data
            if (frame.IsEmpty)
            {
                frame.Dispose();
                return null;
            }

            return frame;
        }

        public void Stop()
        {
            _capture?.Dispose();
            _capture = null;
        }
    }
}