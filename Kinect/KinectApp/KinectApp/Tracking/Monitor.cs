using System;
using KinectApp.Extra;
using NLog;

namespace KinectApp.Tracking
{
    /// <summary>
    /// This class is used to keep track of the previous angle and calculate wether the new angle exceeds the threshold.
    /// </summary>
    public class Monitor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The last angle
        /// </summary>
        public float PreviousAngle { get; set; }

        /// <summary>
        /// The treshold to exceed to notify a change
        /// </summary>
        public float Treshold { get; set; }

        /// <summary>
        /// The bodypart being monitored
        /// </summary>
        public BodyPart BodyPart { get; set; }

        /// <summary>
        /// If true, the monitor will log the angles when the threshold was exceeded
        /// </summary>
        public bool PrintRealtime { get; set; }

        /// <summary>
        /// Checks if the new angle exceeds the treshold for this bodypart. It will automatically update the PreviousAngle.
        /// </summary>
        /// <param name="newAngle">The new angle</param>
        /// <returns>True if the threshold was exceeded</returns>
        public bool ThresholdExceeded(float newAngle)
        {
            var thresholdExceeded = Math.Abs(newAngle - PreviousAngle) > Treshold;
            if (thresholdExceeded)
            {
                PreviousAngle = newAngle;
                if (PrintRealtime)
                {
                    Logger.Info("{0} changed to {1} degrees", BodyPart, RotationHelper.RadianToDegree(newAngle));
                }
            }
            return thresholdExceeded;
        }
    }
}
