using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace KinectApp.Extra
{
    /// <summary>
    /// Helper class for JSON serialization and deserialization
    /// </summary>
    public class JsonHelper
    {

        /// <summary>
        /// JSON serialization for the the given object.
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <param name="t">The object to serialize</param>
        /// <exception cref="SerializationException">This exception is usually caused by trying to use a null value where a null value is not allowed.</exception>
        /// <returns>A JSON string representing the object</returns>
        public static string Serialize<T>(T t)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            var memStream = new MemoryStream();

            using (memStream)
            {
                serializer.WriteObject(memStream, t);
                var jsonString = Encoding.UTF8.GetString(memStream.ToArray());
                return jsonString;
            }
        }


        /// <summary>
        /// JSON deserialization for the the given string.
        /// </summary>
        /// <typeparam name="T">The type of the required object</typeparam>
        /// <param name="jsonString">The JSON string</param>
        /// <exception cref="SerializationException">Thrown when there's a problem deserializing</exception>
        /// <returns>The object created from the JSON</returns>
        public static T Deserialize<T>(string jsonString)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            var memStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            using (memStream)
            {
                var obj = (T)serializer.ReadObject(memStream);
                return obj;
            }
        }
    }
}