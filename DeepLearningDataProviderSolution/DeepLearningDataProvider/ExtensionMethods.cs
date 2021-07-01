using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DeepLearningDataProvider
{
    public static class ExtensionMethods
    {
        internal static List<T> ToList<T>(this Array arr)
        {
            return arr.Cast<T>().ToList();
        }
        internal static void ForEach<T>(this Array source, Action<T> action)
        {
            for (int i = 0; i < source.GetLength(0); i++)
            {
                action((T)source.GetValue(i));
            }
        }
        internal static void ForEach<T, TResult>(this Array source, Func<T, TResult> func)
        {
            for (int i = 0; i < source.GetLength(0); i++)
            {
                source.SetValue(func((T)source.GetValue(i)), i);
            }
        }
        internal static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection)
        {
            Random rnd = new Random();

            T[] result = collection.ToArray();
            int count = collection.Count();

            for (int index = 0; index < count; index++)
            {
                int newIndex = rnd.Next(count);

                T item = result[index];
                result[index] = result[newIndex];
                result[newIndex] = item;
            }

            return result;
        }
        /// <summary>
        /// Only meant for small example data sets. 
        /// Huge sample collections can throw 'OutOfMemory' exception here.
        /// </summary>
        public static string ToJson<T>(this T obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }
        /// <summary>
        /// Only meant for small  example data sets. 
        /// Huge sample collections can throw 'OutOfMemory' exception here.
        /// </summary>
        public static bool Save<T>(this T obj, string file, StorageFormat format)
        {
            switch (format)
            {
                case StorageFormat.Undefined:
                    return false;
                case StorageFormat.ByteArray:
                    throw new NotImplementedException();
                case StorageFormat.Idx3ubyte:
                    throw new NotImplementedException();
                case StorageFormat.Json:
                    try
                    {                        
                        File.WriteAllText(file, obj.ToJson());
                        return true;
                    }
                    catch (Exception)
                    {
                        throw;
                        // return false;
                    }
                default:
                    return false;
            }
        }

        #region https://stackoverflow.com/a/49407977

        public static int ReadBigInt32(this BinaryReader br)
        {
            var bytes = br.ReadBytes(sizeof(int));
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        #endregion
    }
}
