using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.FaceTracking;

namespace KinectApp.Tracking
{
    public class FaceTrackingViewer : IDisposable
    {
        private const uint MaxMissedFrames = 100;

        private readonly Dictionary<int, SkeletonFaceTracker> _trackedSkeletons = new Dictionary<int, SkeletonFaceTracker>();

        private byte[] _colorImage;
        private ColorImageFormat _colorImageFormat = ColorImageFormat.Undefined;

        private short[] _depthImage;
        private DepthImageFormat _depthImageFormat = DepthImageFormat.Undefined;

        private bool _disposed;

        private Skeleton[] _skeletonData;

        private static ChangeTracker _changeTracker;

        public FaceTrackingViewer(KinectSensor kinectSensor, ChangeTracker changeTracker)
        {
            _changeTracker = changeTracker;            
            Kinect = kinectSensor;
        }

        public KinectSensor Kinect { get; private set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                ResetFaceTracking();

                _disposed = true;
            }
        }

        public void OnAllFramesReady(object sender, AllFramesReadyEventArgs allFramesReadyEventArgs)
        {
            ColorImageFrame colorImageFrame = null;
            DepthImageFrame depthImageFrame = null;
            SkeletonFrame skeletonFrame = null;

            try
            {
                colorImageFrame = allFramesReadyEventArgs.OpenColorImageFrame();
                depthImageFrame = allFramesReadyEventArgs.OpenDepthImageFrame();
                skeletonFrame = allFramesReadyEventArgs.OpenSkeletonFrame();

                if (colorImageFrame == null || depthImageFrame == null || skeletonFrame == null)
                {
                    return;
                }

                // Check for image format changes.  The FaceTracker doesn't
                // deal with that so we need to reset.
                if (_depthImageFormat != depthImageFrame.Format)
                {
                    ResetFaceTracking();
                    _depthImage = null;
                    _depthImageFormat = depthImageFrame.Format;
                }

                if (_colorImageFormat != colorImageFrame.Format)
                {
                    ResetFaceTracking();
                    _colorImage = null;
                    _colorImageFormat = colorImageFrame.Format;
                }

                // Create any buffers to store copies of the data we work with
                if (_depthImage == null)
                {
                    _depthImage = new short[depthImageFrame.PixelDataLength];
                }

                if (_colorImage == null)
                {
                    _colorImage = new byte[colorImageFrame.PixelDataLength];
                }

                // Get the skeleton information
                if (_skeletonData == null || _skeletonData.Length != skeletonFrame.SkeletonArrayLength)
                {
                    _skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                }

                colorImageFrame.CopyPixelDataTo(_colorImage);
                depthImageFrame.CopyPixelDataTo(_depthImage);
                skeletonFrame.CopySkeletonDataTo(_skeletonData);

                // Update the list of trackers and the trackers with the current frame information
                foreach (Skeleton skeleton in _skeletonData)
                {
                    if (skeleton.TrackingState == SkeletonTrackingState.Tracked
                        || skeleton.TrackingState == SkeletonTrackingState.PositionOnly)
                    {
                        // We want keep a record of any skeleton, tracked or untracked.
                        if (!_trackedSkeletons.ContainsKey(skeleton.TrackingId))
                        {
                            _trackedSkeletons.Add(skeleton.TrackingId, new SkeletonFaceTracker());
                        }

                        // Give each tracker the upated frame.
                        SkeletonFaceTracker skeletonFaceTracker;
                        if (_trackedSkeletons.TryGetValue(skeleton.TrackingId, out skeletonFaceTracker))
                        {
                            skeletonFaceTracker.OnFrameReady(Kinect, _colorImageFormat, _colorImage, _depthImageFormat, _depthImage, skeleton);
                            skeletonFaceTracker.LastTrackedFrame = skeletonFrame.FrameNumber;
                        }
                    }
                }

                RemoveOldTrackers(skeletonFrame.FrameNumber);

            }
            finally
            {
                if (colorImageFrame != null)
                {
                    colorImageFrame.Dispose();
                }

                if (depthImageFrame != null)
                {
                    depthImageFrame.Dispose();
                }

                if (skeletonFrame != null)
                {
                    skeletonFrame.Dispose();
                }
            }
        }

        /// <summary>
        /// Clear out any trackers for skeletons we haven't heard from for a while
        /// </summary>
        private void RemoveOldTrackers(int currentFrameNumber)
        {
            var trackersToRemove = new List<int>();

            foreach (var tracker in _trackedSkeletons)
            {
                uint missedFrames = (uint)currentFrameNumber - (uint)tracker.Value.LastTrackedFrame;
                if (missedFrames > MaxMissedFrames)
                {
                    // There have been too many frames since we last saw this skeleton
                    trackersToRemove.Add(tracker.Key);
                }
            }

            foreach (int trackingId in trackersToRemove)
            {
                RemoveTracker(trackingId);
            }
        }

        private void RemoveTracker(int trackingId)
        {
            _trackedSkeletons[trackingId].Dispose();
            _trackedSkeletons.Remove(trackingId);
        }

        private void ResetFaceTracking()
        {
            foreach (int trackingId in new List<int>(_trackedSkeletons.Keys))
            {
                RemoveTracker(trackingId);
            }
        }

        private class SkeletonFaceTracker : IDisposable
        {
            private FaceTracker _faceTracker;

            private SkeletonTrackingState _skeletonTrackingState;

            public int LastTrackedFrame { get; set; }

            public void Dispose()
            {
                if (_faceTracker != null)
                {
                    _faceTracker.Dispose();
                    _faceTracker = null;
                }
            }

            /// <summary>
            /// Updates the face tracking information for this skeleton
            /// </summary>
            internal void OnFrameReady(KinectSensor kinectSensor, ColorImageFormat colorImageFormat, byte[] colorImage, DepthImageFormat depthImageFormat, short[] depthImage, Skeleton skeletonOfInterest)
            {
                _skeletonTrackingState = skeletonOfInterest.TrackingState;

                if (_skeletonTrackingState != SkeletonTrackingState.Tracked)
                {
                    // nothing to do with an untracked skeleton.
                    return;
                }

                if (_faceTracker == null)
                {
                    try
                    {
                        _faceTracker = new FaceTracker(kinectSensor);
                    }
                    catch (InvalidOperationException)
                    {
                        // During some shutdown scenarios the FaceTracker
                        // is unable to be instantiated.  Catch that exception
                        // and don't track a face.
                        Debug.WriteLine("AllFramesReady - creating a new FaceTracker threw an InvalidOperationException");
                        _faceTracker = null;
                    }
                }

                if (_faceTracker != null)
                {
                    FaceTrackFrame faceFrame = _faceTracker.Track(
                                colorImageFormat, colorImage, depthImageFormat, depthImage, skeletonOfInterest);

                    // Only works if face is detected
                    // Head Roll: faceFrame.Rotation.Z
                    // Head Pitch: faceFrame.Rotation.X
                    // Head Yaw: faceFrame.Rotation.Y              
                    if (faceFrame.TrackSuccessful)
                    {
                        _changeTracker.AngleCalculator.FaceRotation = faceFrame.Rotation;
                        _changeTracker.CheckChanges(skeletonOfInterest);
                    }
                }
            }
        }
    }
}
