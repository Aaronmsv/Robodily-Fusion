using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media.Media3D;
using KinectApp.Extra;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.FaceTracking;

namespace KinectApp.Tracking
{
    public class AngleCalculator
    {
        // Todo handle NaN exceptions correctly
        #region Joint collections
        //Below are collections of joints needed to calculate a certain movement
        //KNEE
        private static readonly JointType[] LeftKneePitchJoints =
        {
            JointType.HipLeft, JointType.KneeLeft,
            JointType.AnkleLeft
        };

        private static readonly JointType[] RightKneePitchJoints =
        {
            JointType.HipRight, JointType.KneeRight,
            JointType.AnkleRight
        };

        //HIP
        private static readonly JointType[] LeftHipPitchJoints = { JointType.Spine, JointType.HipLeft, JointType.KneeLeft };
        private static readonly JointType[] RightHipPitchJoints = { JointType.Spine, JointType.HipRight, JointType.KneeRight };

        //ANKLE
        private static readonly JointType[] LeftAnklePitchJoints =
        {
            JointType.KneeLeft, JointType.AnkleLeft,
            JointType.FootLeft
        };

        private static readonly JointType[] RightAnklePitchJoints =
        {
            JointType.KneeRight, JointType.AnkleRight,
            JointType.FootRight
        };

        #endregion

        public Vector3DF FaceRotation { get; set; }

        /// <summary>
        /// Returns the angle of a bodypart for a given skeleton.
        /// </summary>
        /// <param name="skeleton">The skeleton</param>
        /// <param name="bodyPart">The bodypart</param>
        /// <returns>The angle in radians of the requested bodypart for the given skeleton.
        /// Throws InvalidEnumArgumentException if the bodypart is not now.</returns>
        public float GetAngle(Skeleton skeleton, BodyPart bodyPart)
        {
            switch (bodyPart)
            {
                //head
                case BodyPart.HeadPitch:
                    return GetHeadPitch();
                case BodyPart.HeadRoll:
                    return GetHeadRoll();
                case BodyPart.HeadYaw:
                    return GetHeadYaw();
                case BodyPart.LeftAnklePitch:
                    return GetLeftAnklePitch(skeleton);
                case BodyPart.LeftAnkleRoll:
                    //todo
                    break;
                case BodyPart.LeftElbowRoll:
                    return GetLeftElbowRoll(skeleton);
                case BodyPart.LeftElbowYaw:
                    return GetLeftElbowYaw(skeleton);
                case BodyPart.LeftHipPitch:
                    return GetLeftHipPitch(skeleton);
                case BodyPart.LeftHipRoll:
                    //todo
                    break;
                case BodyPart.LeftHipYawPitch:
                    //todo
                    break;
                case BodyPart.LeftKneePitch:
                    return GetLeftKneePitch(skeleton);
                case BodyPart.LeftShoulderPitch:
                    return GetLeftShoulderPitch(skeleton);
                case BodyPart.LeftShoulderRoll:
                    return GetLeftShoulderRoll(skeleton);
                case BodyPart.LeftWristYaw:
                    //todo
                    break;
                case BodyPart.RightAnklePitch:
                    return GetRightAnklePitch(skeleton);
                case BodyPart.RightAnkleRoll:
                    //return GetRightAnkleRoll(skeleton);
                    //todo
                    break;
                case BodyPart.RightElbowRoll:
                    return GetRightElbowRoll(skeleton);
                case BodyPart.RightElbowYaw:
                    return GetRightElbowYaw(skeleton);
                case BodyPart.RightHipPitch:
                    return GetRightHipPitch(skeleton);
                case BodyPart.RightHipRoll:
                    //return GetRHipRollAngle(skeleton);
                    //todo
                    break;
                case BodyPart.RightHipYawPitch:
                    //todo
                    break;
                case BodyPart.RightKneePitch:
                    return GetRightKneePitch(skeleton);
                case BodyPart.RightShoulderPitch:
                    return GetRightShoulderPitch(skeleton);
                case BodyPart.RightShoulderRoll:
                    return GetRightShoulderRoll(skeleton);
                case BodyPart.RightWristYaw:
                    //todo
                    break;
            }
            throw new InvalidEnumArgumentException();
        }

        #region Vector helper methods

        /// <summary>
        /// Maps a joint to a vector
        /// </summary>
        /// <param name="joint">The joint</param>
        /// <returns>A vector defining the joint</returns>
        private static Vector3D JointToVector(Joint joint)
        {
            return new Vector3D(joint.Position.X, joint.Position.Y, joint.Position.Z);
        }

        /// <summary>
        /// Projects a vector on a plane 
        /// </summary>
        /// <param name="planeNormal">The plane</param>
        /// <param name="v">The vector</param>
        /// <returns>The projection of that vector on the given plane</returns>
        private static Vector3D ProjectToPlane(Vector3D planeNormal, Vector3D v)
        {
            return v - (Vector3D.DotProduct(v, planeNormal) * planeNormal);
        }

        /// <summary>
        /// Inverts the angle 
        /// </summary>
        /// <param name="angle">The angle to invert</param>
        /// <returns>The inverted angle</returns>
        private static float InvertAngle(float angle)
        {
            return (float)(2 * Math.PI - angle);
        }

        #endregion

        #region Elbow roll

        /// <summary>
        /// Returns the angle betweens two vectors, default in radians
        /// </summary>
        /// <param name="vectorA">The first vector</param>
        /// <param name="vectorB">The second vector</param>
        /// <param name="inDegrees">Returns in degrees if true, default is false</param>
        /// <returns>The angle betweens two vectors in radians</returns>
        private static float AngleBetweenTwoVectors(Vector3D vectorA, Vector3D vectorB, bool inDegrees = false)
        {
            vectorA.Normalize();
            vectorB.Normalize();
            var dotProduct = Vector3D.DotProduct(vectorA, vectorB);
            var radians = Math.Acos(dotProduct);

            //if output requested in degrees
            if (inDegrees)
            {
                return (float)(radians / Math.PI * 180);
            }

            return (float)radians;
        }

        /// <summary>
        /// Calculates the angle of the right elbow
        /// </summary>
        /// <param name="skeleton">The skeleton</param>
        /// <returns>The elbow angle in radians</returns>
        public float GetRightElbowRoll(Skeleton skeleton)
        {
            var rightShoulder = JointToVector(skeleton.Joints[JointType.ShoulderRight]);
            var rightElbow = JointToVector(skeleton.Joints[JointType.ElbowRight]);
            var rightWrist = JointToVector(skeleton.Joints[JointType.WristRight]);
            var rightElbowAngle = AngleBetweenTwoVectors(rightElbow - rightShoulder, rightElbow - rightWrist);
            return rightElbowAngle;
        }

        /// <summary>
        /// Calculates the angle of the left elbow
        /// </summary>
        /// <param name="skeleton">The skeleton</param>
        /// <returns>The elbow angle in radians</returns>
        public float GetLeftElbowRoll(Skeleton skeleton)
        {
            var leftShoulder = JointToVector(skeleton.Joints[JointType.ShoulderLeft]);
            var leftElbow = JointToVector(skeleton.Joints[JointType.ElbowLeft]);
            var leftWrist = JointToVector(skeleton.Joints[JointType.WristLeft]);
            var leftElbowAngle = AngleBetweenTwoVectors(leftElbow - leftShoulder, leftElbow - leftWrist);
            return leftElbowAngle;
        }

        #endregion

        #region Shoulder roll

        /// <summary>
        /// Calculates the shoulder roll angle
        /// </summary>
        /// <param name="shoulder">The shoulder</param>
        /// <param name="elbow">The elbow</param>
        /// <returns>The angle between shoulder and elbow (radians)</returns>
        private float CalculateShoulderRollAngle(SkeletonPoint shoulder, SkeletonPoint elbow)
        {
            var shoulderToElbowVector = new Vector3D(shoulder.X - elbow.X, shoulder.Y - elbow.Y, shoulder.Z - elbow.Z);
            float shoulderRollAngle;

            if (shoulderToElbowVector.Y >= 0)
            {
                shoulderRollAngle = (float)Math.Atan2(shoulderToElbowVector.X, shoulderToElbowVector.Y);
            }
            else
            {
                shoulderRollAngle = (float)Math.Atan2(shoulderToElbowVector.X, -1 * shoulderToElbowVector.Y);
            }
            return shoulderRollAngle;
        }

        /// <summary>
        /// Calculates the left shoulder roll
        /// </summary>
        /// <param name="skeleton">The skeleton</param>
        /// <returns>The angle in radians</returns>
        public float GetLeftShoulderRoll(Skeleton skeleton)
        {
            var shoulderLeftPosition = skeleton.Joints[JointType.ShoulderLeft].Position;
            var elbowLeftPosition = skeleton.Joints[JointType.ElbowLeft].Position;

            var shoulderRollAngle = CalculateShoulderRollAngle(shoulderLeftPosition, elbowLeftPosition);

            if (shoulderRollAngle < 0) // TODO: check this
            {
                shoulderRollAngle = 0;
            }

            return shoulderRollAngle;
        }

        /// <summary>
        /// Calculates the right shoulder roll
        /// </summary>
        /// <param name="skeleton">The skeleton</param>
        /// <returns>The angle in radians</returns>
        public float GetRightShoulderRoll(Skeleton skeleton)
        {
            var shoulderRightPosition = skeleton.Joints[JointType.ShoulderRight].Position;
            var elbowRightPosition = skeleton.Joints[JointType.ElbowRight].Position;

            var shoulderRollAngle = CalculateShoulderRollAngle(shoulderRightPosition, elbowRightPosition);

            if (shoulderRollAngle > 0) // TODO: check this
            {
                shoulderRollAngle = 0;
            }

            return shoulderRollAngle;
        }

        #endregion

        #region Shoulder pitch

        /// <summary>
        /// Calculates the angle of the right shoulder pitch
        /// </summary>
        /// <param name="skeleton">The skeleton</param>
        /// <returns>The angle of the shoulder pitch (in radians)</returns>
        public float GetRightShoulderPitch(Skeleton skeleton)
        {
            return CalculateShoulderPitch(skeleton, JointType.ShoulderRight, JointType.ElbowRight);
        }

        /// <summary>
        /// Calculates the angle of the left shoulder pitch
        /// </summary>
        /// <param name="skeleton">The skeleton</param>
        /// <returns>The angle of the shoulder pitch (in radians)</returns>
        public float GetLeftShoulderPitch(Skeleton skeleton)
        {
            return CalculateShoulderPitch(skeleton, JointType.ShoulderLeft, JointType.ElbowLeft);
        }

        /// <summary>
        /// Calculates the shoulder pitch angle
        /// </summary>
        /// <param name="skeleton">The skeleton</param>
        /// <param name="shoulder">The shoulder joint</param>
        /// <param name="elbow">The elbow joint</param>
        /// <returns>The shoulder pitch angle in radians</returns>
        private float CalculateShoulderPitch(Skeleton skeleton, JointType shoulder, JointType elbow)
        {
            // Vector spine -> shoulder
            var shoulderCenter = JointToVector(skeleton.Joints[JointType.ShoulderCenter]);
            var spine = JointToVector(skeleton.Joints[JointType.Spine]);
            var spineToCenter = spine - shoulderCenter;
            spineToCenter.Normalize();

            // Vector shoulder -> shoulder center
            var shoulderRight = JointToVector(skeleton.Joints[shoulder]);
            var shoulderToCenter = shoulderRight - shoulderCenter;
            shoulderToCenter.Normalize();

            // A vector perpendicular to the chest plane
            var chestPlaneVector = Vector3D.CrossProduct(shoulderToCenter, -spineToCenter);

            // Vector elbow -> shoulder
            var elbowRight = JointToVector(skeleton.Joints[elbow]);
            var elbowToShoulder = elbowRight - shoulderRight;

            // A vector perpendicular on the plane through elbow-shoulder and shoulder-center 
            var shoulderPitchPlaneVector = Vector3D.CrossProduct(elbowToShoulder, shoulderToCenter);

            // Projection of the elbowToShoulder vector on the shoulderPitchPlaneVector
            var pitchVector = ProjectToPlane(shoulderPitchPlaneVector, elbowToShoulder);

            // Projection of the elbowToShoulder vector on the shoulderToCenter vector
            var yawVector = ProjectToPlane(shoulderToCenter, elbowToShoulder);

            float shoulderYaw = 0;
            if (yawVector.Length > 0.5)
            {
                // Only if jaw isn't too small
                shoulderYaw = (float)Vector3D.AngleBetween(yawVector, -chestPlaneVector);

                if (Vector3D.DotProduct(yawVector, spineToCenter) < 0)
                {
                    // Inverting shoulder yaw if dot product is negative
                    shoulderYaw = InvertAngle(shoulderYaw);
                }
            }

            var yawRotation = new AxisAngleRotation3D(shoulderToCenter, shoulderYaw);
            var rotateTransform3D = new RotateTransform3D(yawRotation);
            var downInPitchFrame = rotateTransform3D.Transform(spineToCenter);

            // Check if one the vectors is just the origin => will result in NaN otherwise
            if ((pitchVector.X == 0f && pitchVector.Y == 0 && pitchVector.Z == 0) ||
                (downInPitchFrame.X == 0f && downInPitchFrame.Y == 0 && downInPitchFrame.Z == 0))
            {
                return 0;
            }

            return AngleBetweenTwoVectors(pitchVector, downInPitchFrame);
        }

        #endregion

        #region Elbow yaw

        // Todo 0 and 180 degrees need to be switched

        /// <summary>
        /// Calculates the elbow yaw angle
        /// </summary>
        /// <param name="skeleton">The skeleton</param>
        /// <param name="shoulder">The shoulder joint</param>
        /// <param name="elbow">The elbow joint</param>
        /// <param name="wrist">The wrist joint</param>
        /// <returns>The elbow yaw angle in radians</returns>
        private float CalculateElbowYaw(Skeleton skeleton, JointType shoulder, JointType elbow, JointType wrist)
        {
            // The vectors
            var shoulderLeft = JointToVector(skeleton.Joints[shoulder]);
            var hipCenter = JointToVector(skeleton.Joints[JointType.HipCenter]);
            var leftElbow = JointToVector(skeleton.Joints[elbow]);
            var leftWrist = JointToVector(skeleton.Joints[wrist]);

            // Computed vectors
            var shoulderToElbow = shoulderLeft - leftElbow;
            var elbowToWrist = leftElbow - leftWrist;
            var shoulderToHipCenter = shoulderLeft - hipCenter;

            // Angle calculation
            var crossCenterArm = Vector3D.CrossProduct(shoulderToHipCenter, shoulderToElbow);
            var crossArms = Vector3D.CrossProduct(shoulderToElbow, elbowToWrist);
            return AngleBetweenTwoVectors(crossCenterArm, crossArms);
        }

        /// <summary>
        /// Calculates the right elbow yaw
        /// </summary>
        /// <param name="skeleton">The skeleton</param>
        /// <returns>The right elbow yaw in radians</returns>
        public float GetRightElbowYaw(Skeleton skeleton)
        {
            return CalculateElbowYaw(skeleton, JointType.ShoulderRight, JointType.ElbowRight, JointType.WristRight);
        }

        /// <summary>
        /// Calculates the left elbow yaw
        /// </summary>
        /// <param name="skeleton">The skeleton</param>
        /// <returns>The left elbow yaw in radians</returns>
        public float GetLeftElbowYaw(Skeleton skeleton)
        {
            return CalculateElbowYaw(skeleton, JointType.ShoulderLeft, JointType.ElbowLeft, JointType.WristLeft);
        }

        #endregion

        #region Knee Pitch

        /// <summary>
        /// Calculates the angle between 3 joints
        /// </summary>
        /// <param name="joints">The joints (should be 3)</param>
        /// <param name="skeleton">The skeleton</param>
        /// <returns>The angle between the 3 joints in radians</returns>
        private static float GetPitchAngle(IList<JointType> joints, Skeleton skeleton)
        {
            var vectorA = JointToVector(skeleton.Joints[joints[0]]);
            var vectorB = JointToVector(skeleton.Joints[joints[1]]);
            var vectorC = JointToVector(skeleton.Joints[joints[2]]);
            return AngleBetweenTwoVectors(vectorB - vectorA, vectorB - vectorC);
        }

        /// <summary>
        /// Calculates the right knee pitch
        /// </summary>
        /// <param name="skeleton">The skeleton</param>
        /// <returns>The right knee pitch in radians</returns>
        public float GetRightKneePitch(Skeleton skeleton)
        {
            return GetPitchAngle(RightKneePitchJoints, skeleton);
        }

        /// <summary>
        /// Calculates the left knee pitch
        /// </summary>
        /// <param name="skeleton">The skeleton</param>
        /// <returns>The left knee pitch in radians</returns>
        public float GetLeftKneePitch(Skeleton skeleton)
        {
            return GetPitchAngle(LeftKneePitchJoints, skeleton);
        }

        #endregion

        #region Hip pitch

        /// <summary>
        /// Calculates the right hip pitch
        /// </summary>
        /// <param name="skeleton">The skeleton</param>
        /// <returns>The right hip pitch in radians</returns>
        public float GetRightHipPitch(Skeleton skeleton)
        {
            return GetPitchAngle(RightHipPitchJoints, skeleton);
        }

        /// <summary>
        /// Calculates the left hip pitch
        /// </summary>
        /// <param name="skeleton">The skeleton</param>
        /// <returns>The left hip pitch in radians</returns>
        public float GetLeftHipPitch(Skeleton skeleton)
        {
            return GetPitchAngle(LeftHipPitchJoints, skeleton);
        }

        #endregion

        #region Ankle pitch

        /// <summary>
        /// Calculates the right ankle pitch
        /// </summary>
        /// <param name="skeleton">The skeleton</param>
        /// <returns>The right ankle pitch in radians</returns>
        public float GetRightAnklePitch(Skeleton skeleton)
        {
            return GetPitchAngle(RightAnklePitchJoints, skeleton);
        }

        /// <summary>
        /// Calculates the left ankle pitch
        /// </summary>
        /// <param name="skeleton">The skeleton</param>
        /// <returns>The left ankle pitch in radians</returns>
        public float GetLeftAnklePitch(Skeleton skeleton)
        {
            return GetPitchAngle(LeftAnklePitchJoints, skeleton);
        }

        #endregion

        #region Ankle roll

        /// <summary>
        /// Calculates the right ankle roll
        /// </summary>
        /// <param name="skeleton">The skeleton</param>
        /// <returns>The ankle roll in radians</returns>
        public float GetRightAnkleRoll(Skeleton skeleton)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calculates the left ankle roll
        /// </summary>
        /// <param name="skeleton">The skeleton</param>
        /// <returns>The ankle roll in radians</returns>
        public float GetLeftAnkleRoll(Skeleton skeleton)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Head calculations

        /// <summary>
        /// A helper method to get the head pitch
        /// </summary>
        /// <returns>The head pitch angle in radians</returns>
        public float GetHeadPitch()
        {
            return RotationHelper.DegreeToRadian(FaceRotation.X);
        }

        /// <summary>
        /// A helper method to get the head yaw
        /// </summary>
        /// <returns>The head yaw angle in radians</returns>
        public float GetHeadYaw()
        {
            return RotationHelper.DegreeToRadian(FaceRotation.Y);
        }

        /// <summary>
        /// A helper method to get the head roll
        /// </summary>
        /// <returns>The head roll angle in radians</returns>
        public float GetHeadRoll()
        {
            return RotationHelper.DegreeToRadian(FaceRotation.Z);
        }
        #endregion
    }
}
