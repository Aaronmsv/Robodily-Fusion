using System;
using System.Collections.Generic;
using System.ComponentModel;
using KinectApp.Robots;
using Microsoft.Kinect;
using NLog;

namespace KinectApp.Tracking
{
    /// <summary>
    /// This class will monitor the movements of the user and ask to mappers to create instructions when needed
    /// </summary>
    public class ChangeTracker
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The list of instruction
        /// </summary>
        public IList<Instruction> Instructions { get; private set; }

        /// <summary>
        /// The mapper that will handle creating the instructions
        /// </summary>
        public IRobotMapper RobotMapper { get; set; }
        private readonly AngleCalculator _angleCalculator;
        private readonly Monitor[] _monitors;
        private DateTime _timeStarted;

        #region Boundaries and configuration

        private const float Threshold = 0.25f; // 5% change required
        private const float ShoulderPitchTreshold = Threshold; //(170 - 20) * 0.05f; // min 20 degrees to 170 degrees
        private const float HeadPitchTreshold = 0.08727f; //todo
        private const float HeadYawTreshold = 0.08727f; //todo
        private const float AnklePitchTreshold = 0.17453f; //todo
        private const float AnkleRollTreshold = Threshold; //todo
        private const float ElbowRollTreshold = 0.17453f; //todo
        private const float ElbowYawTreshold = Threshold; //todo
        private const float HandTreshold = Threshold; //todo
        private const float HipPitchTreshold = Threshold; //todo
        private const float HipRollTreshold = Threshold; //todo
        private const float HipYawPitchTreshold = Threshold; //todo
        private const float KneePitchTreshold = Threshold; //todo
        private const float ShoulderRollTreshold = Threshold; //todo
        private const float WristYawTreshold = Threshold; //todo
        private const float HeadRollThreshold = Threshold; //todo

        #endregion

        public AngleCalculator AngleCalculator
        {
            get { return _angleCalculator; }
        }

        /// <summary>
        /// Creates a new ChangeTracker with the IRobotMapper handling the creating of instructions for this specific robot
        /// </summary>
        /// <param name="robotMapper">The robot instruction mapper</param>
        public ChangeTracker(IRobotMapper robotMapper)
        {
            RobotMapper = robotMapper;
            Instructions = new List<Instruction>();
            _angleCalculator = new AngleCalculator();
            _monitors = new[]
            {
                //new Monitor {BodyPart = BodyPart.HeadPitch, Treshold = HeadPitchTreshold},
                //new Monitor {BodyPart = BodyPart.HeadYaw, Treshold = HeadYawTreshold},
                //new Monitor {BodyPart = BodyPart.HeadRoll, Treshold = HeadRollThreshold},

                //new Monitor {BodyPart = BodyPart.LeftElbowRoll, Treshold = ElbowRollTreshold},
                //new Monitor {BodyPart = BodyPart.LeftElbowYaw, Treshold = ElbowYawTreshold},
                //new Monitor {BodyPart = BodyPart.LeftKneePitch, Treshold = KneePitchTreshold},
                //new Monitor {BodyPart = BodyPart.LeftShoulderPitch, Treshold = ShoulderPitchTreshold},
                //new Monitor {BodyPart = BodyPart.LeftShoulderRoll, Treshold = ShoulderRollTreshold},
                //new Monitor {BodyPart = BodyPart.LeftAnklePitch, Treshold = AnklePitchTreshold},
                //new Monitor {BodyPart = BodyPart.LeftAnkleRoll, Treshold = AnkleRollTreshold},
                //new Monitor {BodyPart = BodyPart.LeftHipPitch, Treshold = HipPitchTreshold},
                //new Monitor {BodyPart = BodyPart.LeftHipRoll, Treshold = HipRollTreshold},
                //new Monitor {BodyPart = BodyPart.LeftHipYawPitch, Treshold = HipYawPitchTreshold},
                //new Monitor {BodyPart = BodyPart.LeftWristYaw, Treshold = WristYawTreshold},

                new Monitor {BodyPart = BodyPart.RightElbowRoll, Treshold = ElbowRollTreshold},
                new Monitor {BodyPart = BodyPart.RightElbowYaw, Treshold = ElbowYawTreshold},
                //new Monitor {BodyPart = BodyPart.RightKneePitch, Treshold = KneePitchTreshold},
                new Monitor {BodyPart = BodyPart.RightShoulderPitch, Treshold = ShoulderPitchTreshold},
                new Monitor {BodyPart = BodyPart.RightShoulderRoll, Treshold = ShoulderRollTreshold},
                //new Monitor {BodyPart = BodyPart.RightAnklePitch, Treshold = AnklePitchTreshold},
                //new Monitor {BodyPart = BodyPart.RightAnkleRoll, Treshold = AnkleRollTreshold},
                //new Monitor {BodyPart = BodyPart.RightHipPitch, Treshold = HipPitchTreshold},
                //new Monitor {BodyPart = BodyPart.RightHipRoll, Treshold = HipRollTreshold},
                //new Monitor {BodyPart = BodyPart.RightHipYawPitch, Treshold = HipYawPitchTreshold},
                //new Monitor {BodyPart = BodyPart.RightWristYaw, Treshold = WristYawTreshold}
            };
        }

        /// <summary>
        /// Call this ONCE before calling calling CheckChanges. It will set the record time.
        /// </summary>
        public void StartRecording()
        {
            _timeStarted = DateTime.Now;
        }

        /// <summary>
        /// Will check the skeleton for changes
        /// </summary>
        /// <param name="skeleton">The skeleton</param>
        public void CheckChanges(Skeleton skeleton)
        {
            // Check if StartRecording was called
            if (_timeStarted == DateTime.MinValue)
            {
                throw new InvalidOperationException("StartRecording() method was not called first");
            }

            // If recording for more than 5 minutes, stop recording, this is the limit
            var recordTime = DateTime.Now - _timeStarted;
            if (recordTime >= new TimeSpan(0, 5, 0))
            {
                return;
            }

            foreach (var monitor in _monitors)
            {
                try
                {
                    var newAngle = _angleCalculator.GetAngle(skeleton, monitor.BodyPart);
                    if (monitor.ThresholdExceeded(newAngle))
                    {
                        var instruction = RobotMapper.Move(monitor.BodyPart, newAngle);
                        if (instruction != null)
                        {
                            instruction.Time = (float)recordTime.TotalSeconds;
                            Instructions.Add(instruction);
                        }
                    }
                }
                catch (InvalidEnumArgumentException)
                {
                    Logger.Error("Invalid bodypart in monitor (ChangeTracker): {0}", monitor.BodyPart);
                }
            }
        }

        /// <summary>
        /// Clears the list of intructions and resets the time
        /// </summary>
        public void Reset()
        {
            _timeStarted = DateTime.MinValue;
            Instructions.Clear();
        }
    }
}