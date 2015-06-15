using System;
using System.Text;
using Microsoft.Kinect;

namespace KinectApp.Extra
{
    public static class RotationHelper
    {
        private const string MatrixFormat = "{0}\t{1}\t{2}\t{3}\n";
        private const string QuaternionFormat = "W = {0}\nX = {1}\nY = {2}\nZ = {3}";

        #region String representations
        /// <summary>
        /// Returns a visual presentation of the matrix.
        /// Contains a 4x4 row-major matrix containing the joint rotation information in the top left 3x3 and zero for translation. 
        /// </summary>
        /// <param name="matrix">The matrix</param>
        /// <returns>Visual presentation of the matrix</returns>
        public static string Matrix4ToString(Matrix4 matrix)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(string.Format(MatrixFormat, matrix.M11, matrix.M12, matrix.M13, matrix.M14));
            stringBuilder.Append(string.Format(MatrixFormat, matrix.M21, matrix.M22, matrix.M23, matrix.M24));
            stringBuilder.Append(string.Format(MatrixFormat, matrix.M31, matrix.M32, matrix.M33, matrix.M34));
            stringBuilder.Append(string.Format(MatrixFormat, matrix.M41, matrix.M42, matrix.M43, matrix.M44));
            return stringBuilder.ToString();
        }

        /// <summary>
        ///  Show the quaternion information.
        /// </summary>
        /// <param name="quaternion">The quaternion</param>
        /// <returns>All quaternion parameters in one string</returns>
        public static string QuaternionToString(Vector4 quaternion)
        {
            return string.Format(QuaternionFormat, quaternion.W, quaternion.X, quaternion.Y, quaternion.Z);
        }
        #endregion

        #region Randian and float conversions
        /// <summary>
        /// Convert degrees to radian
        /// </summary>
        /// <param name="angle">The angle in degrees</param>
        /// <returns>The angle in radians</returns>
        public static float DegreeToRadian(float angle)
        {
            return (float)(Math.PI * angle / 180.0);
        }

        /// <summary>
        /// Convert radian to degrees
        /// </summary>
        /// <param name="angle">The angle in radian</param>
        /// <returns>The angle in degrees</returns>
        public static float RadianToDegree(float angle)
        {
            return (float)(angle * (180.0 / Math.PI));
        }
        #endregion

    }
}
