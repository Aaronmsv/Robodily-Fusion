using System.Runtime.Serialization;
using KinectApp.Speech;

namespace KinectApp.Tracking
{
    [DataContract]
    public class Instruction
    {
        /// <summary>
        /// The action the robot needs to execute, e.g. move or speak.
        /// </summary>
        [DataMember(IsRequired = true, Order = 0)]
        public Command Command { get; set; }

        /// <summary>
        /// The bodypart to move
        /// </summary>
        [DataMember(EmitDefaultValue = false, Order = 1)]
        public string BodyPart { get; set; }

        /// <summary>
        /// The direction where to move
        /// </summary>
        //TODO: Direction werkt niet voor 0 -> Up
        [DataMember(Order = 2)]
        public NaoSpeechRecognizer.Direction Direction { get; set; }

        /// <summary>
        /// The new angle for the bodypart
        /// </summary>
        [DataMember(EmitDefaultValue = false, Order = 3)]
        public float Angle { get; set; }

        /// <summary>
        /// The text the robot needs to say when the Command is Speech 
        /// </summary>
        [DataMember(EmitDefaultValue = false, Order = 4)]
        public string Text { get; set; }

        /// <summary>
        /// The time that this movement was executed since recording (in seconds)
        /// </summary>
        [DataMember(EmitDefaultValue = false, Order = 5)]
        public float Time { get; set; }
    }
}
