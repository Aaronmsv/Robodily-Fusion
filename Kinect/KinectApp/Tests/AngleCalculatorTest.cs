using System;
using KinectApp.Tracking;
using Microsoft.Kinect;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    /// <summary>
    /// This class will test the angle calculations from the AngleCalculator class. The spine is always the origin.
    /// These are basic tests to test basic calculations, it's possible that some angles might be wrong when the whole body is in an "unnatural" state.
    /// Here we assume the person is standing up right, how a human stands most of the times. It might be wrong when the person is laying in a sofa for example.
    /// This is not tested and those tests would be much more complex.
    /// </summary>
    [TestClass]
    public class AngleCalculatorTest
    {
        private AngleCalculator _angleCalculator;
        private Skeleton _skeleton;

        [TestInitialize]
        public void InitTest()
        {
            _angleCalculator = new AngleCalculator();
            _skeleton = new Skeleton();
        }

        private static void SetJointPosition(Skeleton skeleton, JointType jointType, float x, float y, float z)
        {
            var joint = skeleton.Joints[jointType];
            joint.Position = new SkeletonPoint { X = x, Y = y, Z = z };
            skeleton.Joints[jointType] = joint;
        }

        #region ElbowRoll tests

        [TestMethod]
        public void TestRightElbowRoll()
        {
            // 0 degrees
            SetJointPosition(_skeleton, JointType.WristRight, 10, 10, 1);
            SetJointPosition(_skeleton, JointType.ElbowRight, 10, 0, 0);
            SetJointPosition(_skeleton, JointType.ShoulderRight, 10, 10, 0);
            Assert.AreEqual(0, _angleCalculator.GetRightElbowRoll(_skeleton), 0.1);

            // 90 degrees
            SetJointPosition(_skeleton, JointType.WristRight, 10, 0, 10);
            SetJointPosition(_skeleton, JointType.ElbowRight, 10, 0, 0);
            SetJointPosition(_skeleton, JointType.ShoulderRight, 10, 10, 0);
            Assert.AreEqual(Math.PI / 2, _angleCalculator.GetRightElbowRoll(_skeleton), 0.01);

            // 180 degrees
            SetJointPosition(_skeleton, JointType.WristRight, 10, -10, 0);
            SetJointPosition(_skeleton, JointType.ElbowRight, 10, 0, 0);
            SetJointPosition(_skeleton, JointType.ShoulderRight, 10, 10, 0);
            Assert.AreEqual(Math.PI, _angleCalculator.GetRightElbowRoll(_skeleton), 0.01);
        }

        [TestMethod]
        public void TestLeftElbowRoll()
        {
            // 0 degrees
            SetJointPosition(_skeleton, JointType.WristLeft, -10, 10, 1);
            SetJointPosition(_skeleton, JointType.ElbowLeft, -10, 0, 0);
            SetJointPosition(_skeleton, JointType.ShoulderLeft, -10, 10, 0);
            Assert.AreEqual(0, _angleCalculator.GetLeftElbowRoll(_skeleton), 0.1);

            // 90 degrees
            SetJointPosition(_skeleton, JointType.WristLeft, -10, 0, 10);
            SetJointPosition(_skeleton, JointType.ElbowLeft, -10, 0, 0);
            SetJointPosition(_skeleton, JointType.ShoulderLeft, -10, 10, 0);
            Assert.AreEqual(Math.PI / 2, _angleCalculator.GetLeftElbowRoll(_skeleton), 0.01);

            // 180 degrees
            SetJointPosition(_skeleton, JointType.WristLeft, -10, -10, 0);
            SetJointPosition(_skeleton, JointType.ElbowLeft, -10, 0, 0);
            SetJointPosition(_skeleton, JointType.ShoulderLeft, -10, 10, 0);
            Assert.AreEqual(Math.PI, _angleCalculator.GetLeftElbowRoll(_skeleton), 0.01);
        }

        #endregion

        #region ShoulderRoll tests

        [TestMethod]
        public void TestRightShoulderRoll()
        {
            // 0 degrees
            SetJointPosition(_skeleton, JointType.ShoulderRight, 10, 10, 0);
            SetJointPosition(_skeleton, JointType.ElbowRight, 10, 0, 0);
            Assert.AreEqual(0, _angleCalculator.GetRightShoulderRoll(_skeleton), 0.01);
            // 45 degrees
            SetJointPosition(_skeleton, JointType.ShoulderRight, 10, 10, 0);
            SetJointPosition(_skeleton, JointType.ElbowRight, 15, 5, 0);
            Assert.AreEqual(Math.PI / 4, Math.Abs(_angleCalculator.GetRightShoulderRoll(_skeleton)), 0.01);

            // 90 degrees
            SetJointPosition(_skeleton, JointType.ShoulderRight, 10, 10, 0);
            SetJointPosition(_skeleton, JointType.ElbowRight, 20, 10, 0);
            Assert.AreEqual(Math.PI / 2, Math.Abs(_angleCalculator.GetRightShoulderRoll(_skeleton)), 0.01);
        }

        [TestMethod]
        public void TestLeftShoulderRoll()
        {
            // 0 degrees
            SetJointPosition(_skeleton, JointType.ShoulderLeft, -10, 10, 0);
            SetJointPosition(_skeleton, JointType.ElbowLeft, -10, 0, 0);
            Assert.AreEqual(0, _angleCalculator.GetLeftShoulderRoll(_skeleton), 0.01);

            // 45 degrees
            SetJointPosition(_skeleton, JointType.ShoulderLeft, -10, 10, 0);
            SetJointPosition(_skeleton, JointType.ElbowLeft, -15, 5, 0);
            Assert.AreEqual(Math.PI / 4, Math.Abs(_angleCalculator.GetLeftShoulderRoll(_skeleton)), 0.01);

            // 90 degrees
            SetJointPosition(_skeleton, JointType.ShoulderLeft, -10, 10, 0);
            SetJointPosition(_skeleton, JointType.ElbowLeft, -20, 10, 0);
            Assert.AreEqual(Math.PI / 2, Math.Abs(_angleCalculator.GetLeftShoulderRoll(_skeleton)), 0.01);
        }

        #endregion

        #region ShoulderPitch tests

        [TestMethod]
        public void TestRightShoulderPitch()
        {
            // 0 degrees
            SetJointPosition(_skeleton, JointType.ShoulderCenter, 0, 10, 0);
            SetJointPosition(_skeleton, JointType.Spine, 0, 0, 0);
            SetJointPosition(_skeleton, JointType.ShoulderRight, 10, 0, 0);
            SetJointPosition(_skeleton, JointType.ElbowRight, 10, 0, 0);
            Assert.AreEqual(0, _angleCalculator.GetRightShoulderPitch(_skeleton), 0.01);

            // 90 degrees
            SetJointPosition(_skeleton, JointType.ShoulderCenter, 0, 10, 0);
            SetJointPosition(_skeleton, JointType.Spine, 0, 0, 0);
            SetJointPosition(_skeleton, JointType.ShoulderRight, 10, 10, 0);
            SetJointPosition(_skeleton, JointType.ElbowRight, 10, 10, 10);
            Assert.AreEqual(Math.PI / 2, _angleCalculator.GetRightShoulderPitch(_skeleton), 0.01);

            // 180 degrees
            SetJointPosition(_skeleton, JointType.ShoulderCenter, 0, 10, 0);
            SetJointPosition(_skeleton, JointType.Spine, 0, 0, 0);
            SetJointPosition(_skeleton, JointType.ShoulderRight, 10, 10, 0);
            SetJointPosition(_skeleton, JointType.ElbowRight, 10, 20, 0);
            Assert.AreEqual(Math.PI, _angleCalculator.GetRightShoulderPitch(_skeleton), 0.01);
        }

        [TestMethod]
        public void TestLeftShoulderPitch()
        {
            // 0 degrees
            SetJointPosition(_skeleton, JointType.ShoulderCenter, 0, 10, 0);
            SetJointPosition(_skeleton, JointType.Spine, 0, 0, 0);
            SetJointPosition(_skeleton, JointType.ShoulderLeft, -10, 0, 0);
            SetJointPosition(_skeleton, JointType.ElbowLeft, -10, 0, 0);
            Assert.AreEqual(0, _angleCalculator.GetLeftShoulderPitch(_skeleton), 0.01);

            // 90 degrees
            SetJointPosition(_skeleton, JointType.ShoulderCenter, 0, 10, 0);
            SetJointPosition(_skeleton, JointType.Spine, 0, 0, 0);
            SetJointPosition(_skeleton, JointType.ShoulderLeft, -10, 10, 0);
            SetJointPosition(_skeleton, JointType.ElbowLeft, -10, 10, 10);
            Assert.AreEqual(Math.PI / 2, _angleCalculator.GetLeftShoulderPitch(_skeleton), 0.01);

            // 180 degrees
            SetJointPosition(_skeleton, JointType.ShoulderCenter, 0, 10, 0);
            SetJointPosition(_skeleton, JointType.Spine, 0, 0, 0);
            SetJointPosition(_skeleton, JointType.ShoulderLeft, -10, 10, 0);
            SetJointPosition(_skeleton, JointType.ElbowLeft, -10, 20, 0);
            Assert.AreEqual(Math.PI, _angleCalculator.GetLeftShoulderPitch(_skeleton), 0.01);
        }

        #endregion

        #region ElbowYaw tests

        // Todo 0 and 180 degrees need to be switched
        [TestMethod]
        public void TestRightElbowYaw()
        {
            // 0 degrees
            SetJointPosition(_skeleton, JointType.ShoulderRight, 10, 10, 0);
            SetJointPosition(_skeleton, JointType.HipCenter, 0, -10, 0);
            SetJointPosition(_skeleton, JointType.ElbowRight, 10, 0, 0);
            SetJointPosition(_skeleton, JointType.WristRight, 20, 0, 0);
            Assert.AreEqual(0, _angleCalculator.GetRightElbowYaw(_skeleton), 0.01);

            // 90 degrees
            SetJointPosition(_skeleton, JointType.ShoulderRight, 10, 10, 0);
            SetJointPosition(_skeleton, JointType.HipCenter, 0, -10, 0);
            SetJointPosition(_skeleton, JointType.ElbowRight, 10, 0, 0);
            SetJointPosition(_skeleton, JointType.WristRight, 10, 0, 10);
            Assert.AreEqual(Math.PI / 2, _angleCalculator.GetRightElbowYaw(_skeleton), 0.01);

            // 180 degrees
            SetJointPosition(_skeleton, JointType.ShoulderRight, 10, 10, 0);
            SetJointPosition(_skeleton, JointType.HipCenter, 0, -10, 0);
            SetJointPosition(_skeleton, JointType.ElbowRight, 10, 0, 0);
            SetJointPosition(_skeleton, JointType.WristRight, 0, 0, 0);
            Assert.AreEqual(Math.PI, _angleCalculator.GetRightElbowYaw(_skeleton), 0.01);
        }

        [TestMethod]
        public void TestLeftElbowYaw()
        {
            // 0 degrees
            SetJointPosition(_skeleton, JointType.ShoulderLeft, -10, 10, 0);
            SetJointPosition(_skeleton, JointType.HipCenter, 0, -10, 0);
            SetJointPosition(_skeleton, JointType.ElbowLeft, -10, 0, 0);
            //SetJointPosition(_skeleton, JointType.WristLeft, 0, 0, 1);
            SetJointPosition(_skeleton, JointType.WristLeft, -20, 0, 0);
            Assert.AreEqual(0, _angleCalculator.GetLeftElbowYaw(_skeleton), 0.01);

            // 90 degrees
            SetJointPosition(_skeleton, JointType.ShoulderLeft, -10, 10, 0);
            SetJointPosition(_skeleton, JointType.HipCenter, 0, -10, 0);
            SetJointPosition(_skeleton, JointType.ElbowLeft, -10, 0, 0);
            SetJointPosition(_skeleton, JointType.WristLeft, -10, 0, 10);
            Assert.AreEqual(Math.PI / 2, _angleCalculator.GetLeftElbowYaw(_skeleton), 0.01);

            // 180 degrees
            SetJointPosition(_skeleton, JointType.ShoulderLeft, -10, 10, 0);
            SetJointPosition(_skeleton, JointType.HipCenter, 0, -10, 0);
            SetJointPosition(_skeleton, JointType.ElbowLeft, -10, 0, 0);
            //SetJointPosition(_skeleton, JointType.WristLeft, -20, 0, 0);
            SetJointPosition(_skeleton, JointType.WristLeft, 0, 0, 0);
            Assert.AreEqual(Math.PI, _angleCalculator.GetLeftElbowYaw(_skeleton), 0.01);
        }

        #endregion

        #region KneePitch tests

        [TestMethod]
        public void TestRightKneePitch()
        {
            // 0 degrees
            SetJointPosition(_skeleton, JointType.HipRight, 10, -10, 0);
            SetJointPosition(_skeleton, JointType.KneeRight, 10, -20, 0);
            SetJointPosition(_skeleton, JointType.AnkleRight, 10, -10, 0);
            Assert.AreEqual(0, _angleCalculator.GetRightKneePitch(_skeleton), 0.01);

            // 90 degrees
            SetJointPosition(_skeleton, JointType.HipRight, 10, -10, 0);
            SetJointPosition(_skeleton, JointType.KneeRight, 10, -10, 10);
            SetJointPosition(_skeleton, JointType.AnkleRight, 10, -20, 10);
            Assert.AreEqual(Math.PI / 2, _angleCalculator.GetRightKneePitch(_skeleton), 0.01);

            // 180 degrees
            SetJointPosition(_skeleton, JointType.HipRight, 10, -10, 0);
            SetJointPosition(_skeleton, JointType.KneeRight, 10, -20, 0);
            SetJointPosition(_skeleton, JointType.AnkleRight, 10, -30, 0);
            Assert.AreEqual(Math.PI, _angleCalculator.GetRightKneePitch(_skeleton), 0.01);
        }

        [TestMethod]
        public void TestLeftKneePitch()
        {
            // 0 degrees
            SetJointPosition(_skeleton, JointType.HipLeft, -10, -10, 0);
            SetJointPosition(_skeleton, JointType.KneeLeft, -10, -20, 0);
            SetJointPosition(_skeleton, JointType.AnkleLeft, -10, -10, 0);
            Assert.AreEqual(0, _angleCalculator.GetLeftKneePitch(_skeleton), 0.01);

            // 90 degrees
            SetJointPosition(_skeleton, JointType.HipLeft, -10, -10, 0);
            SetJointPosition(_skeleton, JointType.KneeLeft, -10, -10, 10);
            SetJointPosition(_skeleton, JointType.AnkleLeft, -10, -20, 10);
            Assert.AreEqual(Math.PI / 2, _angleCalculator.GetLeftKneePitch(_skeleton), 0.01);

            // 180 degrees
            SetJointPosition(_skeleton, JointType.HipLeft, -10, -10, 0);
            SetJointPosition(_skeleton, JointType.KneeLeft, -10, -20, 0);
            SetJointPosition(_skeleton, JointType.AnkleLeft, -10, -30, 0);
            Assert.AreEqual(Math.PI, _angleCalculator.GetLeftKneePitch(_skeleton), 0.01);
        }

        #endregion

        #region HipPitch tests

        [TestMethod]
        public void TestRightHipPitch()
        {
            // 0 degrees
            SetJointPosition(_skeleton, JointType.Spine, 0, 0, 0);
            SetJointPosition(_skeleton, JointType.HipRight, 10, -10, 0);
            SetJointPosition(_skeleton, JointType.KneeRight, 10, 0, 0);
            // (0,0,0) for KneeRight will result in an angle of 0 radians, but this is not correct.
            Assert.AreEqual(0, _angleCalculator.GetRightHipPitch(_skeleton), 0.01);

            // 90 degrees
            SetJointPosition(_skeleton, JointType.Spine, 0, 0, 0);
            SetJointPosition(_skeleton, JointType.HipRight, 10, -10, 0);
            SetJointPosition(_skeleton, JointType.KneeRight, 10, -10, 10);
            Assert.AreEqual(Math.PI / 2, _angleCalculator.GetRightHipPitch(_skeleton), 0.01);

            // 180 degrees
            SetJointPosition(_skeleton, JointType.Spine, 0, 0, 0);
            SetJointPosition(_skeleton, JointType.HipRight, 10, -10, 0);
            SetJointPosition(_skeleton, JointType.KneeRight, 10, -20, 0);
            Assert.AreEqual(Math.PI, _angleCalculator.GetRightHipPitch(_skeleton), 0.01);
        }

        [TestMethod]
        public void TestLeftHipPitch()
        {
            // 0 degrees
            SetJointPosition(_skeleton, JointType.Spine, 0, 0, 0);
            SetJointPosition(_skeleton, JointType.HipLeft, -10, -10, 0);
            SetJointPosition(_skeleton, JointType.KneeLeft, -10, 0, 0);
            Assert.AreEqual(0, _angleCalculator.GetLeftHipPitch(_skeleton), 0.01);

            // 90 degrees
            SetJointPosition(_skeleton, JointType.Spine, 0, 0, 0);
            SetJointPosition(_skeleton, JointType.HipLeft, -10, -10, 0);
            SetJointPosition(_skeleton, JointType.KneeLeft, -10, -10, 10);
            Assert.AreEqual(Math.PI / 2, _angleCalculator.GetLeftHipPitch(_skeleton), 0.01);

            // 180 degrees
            SetJointPosition(_skeleton, JointType.Spine, 0, 0, 0);
            SetJointPosition(_skeleton, JointType.HipLeft, -10, -10, 0);
            SetJointPosition(_skeleton, JointType.KneeLeft, -10, -20, 0);
            Assert.AreEqual(Math.PI, _angleCalculator.GetLeftHipPitch(_skeleton), 0.01);
        }

        #endregion

        #region AnklePitch tests

        [TestMethod]
        public void TestRightAnklePitch()
        {
            // 45 degrees
            SetJointPosition(_skeleton, JointType.KneeRight, 10, -20, 0);
            SetJointPosition(_skeleton, JointType.AnkleRight, 10, -30, 0);
            SetJointPosition(_skeleton, JointType.FootRight, 10, -25, 5);
            Assert.AreEqual(Math.PI / 4, _angleCalculator.GetRightAnklePitch(_skeleton), 0.01);

            // 90 degrees
            SetJointPosition(_skeleton, JointType.KneeRight, 10, -20, 0);
            SetJointPosition(_skeleton, JointType.AnkleRight, 10, -30, 0);
            SetJointPosition(_skeleton, JointType.FootRight, 10, -30, 10);
            Assert.AreEqual(Math.PI / 2, _angleCalculator.GetRightAnklePitch(_skeleton), 0.01);

            // 135 degrees
            SetJointPosition(_skeleton, JointType.KneeRight, 10, -20, 0);
            SetJointPosition(_skeleton, JointType.AnkleRight, 10, -30, 0);
            SetJointPosition(_skeleton, JointType.FootRight, 10, -35, 5);
            Assert.AreEqual(Math.PI * 3 / 4, _angleCalculator.GetRightAnklePitch(_skeleton), 0.01);
        }

        [TestMethod]
        public void TestLeftAnklePitch()
        {
            // 45 degrees
            SetJointPosition(_skeleton, JointType.KneeLeft, -10, -20, 0);
            SetJointPosition(_skeleton, JointType.AnkleLeft, -10, -30, 0);
            SetJointPosition(_skeleton, JointType.FootLeft, -10, -25, 5);
            Assert.AreEqual(Math.PI / 4, _angleCalculator.GetLeftAnklePitch(_skeleton), 0.01);

            // 90 degrees
            SetJointPosition(_skeleton, JointType.KneeLeft, -10, -20, 0);
            SetJointPosition(_skeleton, JointType.AnkleLeft, -10, -30, 0);
            SetJointPosition(_skeleton, JointType.FootLeft, -10, -30, 10);
            Assert.AreEqual(Math.PI / 2, _angleCalculator.GetLeftAnklePitch(_skeleton), 0.01);

            // 135 degrees
            SetJointPosition(_skeleton, JointType.KneeLeft, -10, -20, 0);
            SetJointPosition(_skeleton, JointType.AnkleLeft, -10, -30, 0);
            SetJointPosition(_skeleton, JointType.FootLeft, -10, -35, 5);
            Assert.AreEqual(Math.PI * 3 / 4, _angleCalculator.GetLeftAnklePitch(_skeleton), 0.01);
        }

        #endregion

        //todo not yet implemented
        #region Ankle roll

        //[TestMethod]
        public void TestRightAnkleRoll()
        {
            // -15 degrees
            SetJointPosition(_skeleton, JointType.KneeRight, 10, -20, 0);
            SetJointPosition(_skeleton, JointType.AnkleRight, 8, -30, 0);
            SetJointPosition(_skeleton, JointType.FootRight, 12, -30, 10);
            Assert.AreEqual(0, _angleCalculator.GetRightAnkleRoll(_skeleton), 0.01);

            // 0 degrees
            SetJointPosition(_skeleton, JointType.KneeRight, 10, -20, 0);
            SetJointPosition(_skeleton, JointType.AnkleRight, 10, -30, 0);
            SetJointPosition(_skeleton, JointType.FootRight, 10, -30, 10);
            Assert.AreEqual(0, _angleCalculator.GetRightAnkleRoll(_skeleton), 0.01);

            // 15 degrees
            SetJointPosition(_skeleton, JointType.KneeRight, 10, -20, 0);
            SetJointPosition(_skeleton, JointType.AnkleRight, 12, -30, 0);
            SetJointPosition(_skeleton, JointType.FootRight, 8, -30, 10);
            Assert.AreEqual(0, _angleCalculator.GetRightAnkleRoll(_skeleton), 0.01);
        }

        //[TestMethod]
        public void TestLeftAnkleRoll()
        {
            Assert.Fail();
        }

        #endregion

    }
}
