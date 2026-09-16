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

            if (_capture == null || !_capture.Read(frame) || frame.IsEmpty)
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