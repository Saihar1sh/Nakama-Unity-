using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace shGames
{
    public static class SerializationUtils
    {

        #region Core - Json Converter
        
        /// <summary>
        /// Using this to convert object to json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string ToJson(object obj)
        {
            return JsonUtility.ToJson(obj);
        }
        private static T FromJson<T>(string obj)
        {
            return JsonUtility.FromJson<T>(obj);
        }
        
        #endregion
        
        public static T Deserialize<T>(string json)
        {
            return FromJson<T>(json);
        }
        
        public static string Serialize<T>(T obj)
        {
            return ToJson(obj);
        }
        
        public static string Serialize(this Vector3 obj)
        {
            float[] arr = new float[3];
            arr[0] = obj.x;
            arr[1] = obj.y;
            arr[2] = obj.z;
            return ToJson(arr);
        }
        public static string Serialize(this Vector2 obj)
        {
            float[] arr = new float[2];
            arr[0] = obj.x;
            arr[1] = obj.y;
            return ToJson(arr);
        }
        public static string Serialize(this Quaternion obj)
        {
            float[] arr = new float[4];
            arr[0] = obj.x;
            arr[1] = obj.y;
            arr[2] = obj.z;
            arr[3] = obj.w;
            return ToJson(arr);
        }
        
        public static Vector3 DeserializeToVector3(this string json)
        {
            float[] arr = FromJson<float[]>(json);
            return new Vector3(arr[0], arr[1], arr[2]);
        }
        public static Vector2 DeserializeToVector2(this string json)
        {
            float[] arr = FromJson<float[]>(json);
            return new Vector2(arr[0], arr[1]);
        }
        public static Quaternion DeserializeToQuaternion(this string json)
        {
            float[] arr = FromJson<float[]>(json);
            return new Quaternion(arr[0], arr[1], arr[2], arr[3]);
        }
    }
}
