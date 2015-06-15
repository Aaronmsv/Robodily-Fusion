using System.Collections.Generic;
using KinectApp.Tracking;

namespace KinectApp.Robots
{
    public class NaoInstructionMapper : IRobotMapper
    {
        private static readonly IDictionary<BodyPart, string> BodyToRobotMap;

        static NaoInstructionMapper()
        {
            BodyToRobotMap = new Dictionary<BodyPart, string>
            {
                {BodyPart.HeadPitch, "HeadPitch"},
                {BodyPart.HeadYaw, "HeadYaw"},
                {BodyPart.LeftShoulderPitch, "LShoulderPitch"},
                {BodyPart.LeftShoulderRoll, "LShoulderRoll"},
                {BodyPart.LeftElbowRoll, "LElbowRoll"},
                {BodyPart.LeftElbowYaw, "LElbowYaw"},
                {BodyPart.LeftWristYaw, "LWristYaw"},
                {BodyPart.LeftHand, "LHand"},
                {BodyPart.LeftHipPitch, "LHipPitch"},
                {BodyPart.LeftHipRoll, "LHipRoll"},
                {BodyPart.LeftHipYawPitch, "LHipYawPitch"},
                {BodyPart.LeftKneePitch, "LKneePitch"},
                {BodyPart.LeftAnklePitch, "LAnklePitch"},
                {BodyPart.LeftAnkleRoll, "LAnkleRoll"},
                {BodyPart.RightShoulderPitch, "RShoulderPitch"},
                {BodyPart.RightShoulderRoll, "RShoulderRoll"},
                {BodyPart.RightElbowRoll, "RElbowRoll"},
                {BodyPart.RightElbowYaw, "RElbowYaw"},
                {BodyPart.RightWristYaw, "RWristYaw"},
                {BodyPart.RightHand, "RHand"},
                {BodyPart.RightHipPitch, "RHipPitch"},
                {BodyPart.RightHipRoll, "RHipRoll"},
                {BodyPart.RightHipYawPitch, "RHipYawPitch"},
                {BodyPart.RightKneePitch, "RKneePitch"},
                {BodyPart.RightAnklePitch, "RAnklePitch"},
                {BodyPart.RightAnkleRoll, "RAnkleRoll"}
            };
        }

        /// <summary>
        /// Creates an instruction for the NAO robot to move the selected body part with the given angle. 
        /// </summary>
        /// <param name="bodyPart">The body part to move.</param>
        /// <param name="radians">The angle in radians</param>
        /// <returns>The instruction or null if the body part is unknown.</returns>
        public Instruction Move(BodyPart bodyPart, float radians)
        {
            if (!BodyToRobotMap.ContainsKey(bodyPart))
            {
                return null;
            }

            var instruction = new Instruction()
            {
                Command = Command.Move,
                BodyPart = BodyToRobotMap[bodyPart],
                Angle = radians
            };
            return instruction;
        }

        /// <summary>
        /// Creates an instruction for the NAO robot to speak.
        /// </summary>
        /// <param name="text">The text to say</param>
        /// <returns>The instruction</returns>
        public Instruction Speak(string text)
        {
            return new Instruction { Command = Command.Speech, Text = text };
        }
    }
}
