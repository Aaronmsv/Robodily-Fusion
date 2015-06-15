using KinectApp.Tracking;

namespace KinectApp.Robots
{
    public interface IRobotMapper
    {
        /// <summary>
        /// Creates an instruction specific for this kind of robot, using the new angle of this joint.
        /// </summary>
        /// <param name="bodyPart">The bodypart whose angle changed</param>
        /// <param name="degrees">The new angle in degrees</param>
        /// <returns>The instruction for the robot</returns>
        Instruction Move(BodyPart bodyPart, float degrees);

        /// <summary>
        /// Creates an instruction specific for this kind of robot to say the giving string.
        /// </summary>
        /// <param name="text">The text the robot has to say.</param>
        /// <returns>The instruction for the robot</returns>
        Instruction Speak(string text);
    }
}
