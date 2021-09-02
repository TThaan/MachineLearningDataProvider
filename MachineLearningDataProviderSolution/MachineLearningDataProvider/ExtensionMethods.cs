using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MachineLearningDataProvider
{
    internal static class ExtensionMethods
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
        public static string ToStringFromCollection<T>(this IEnumerable<T> collection, string separator = ", ", int lineBreakAfter = 0, int spacesInNewLine = 0)
        {
            List<object> collectionWithLineBreaks = Enumerable.Cast<object>(collection.ToList()).ToList();

            if (lineBreakAfter > 0)
            {
                collectionWithLineBreaks = collection.Select((x, i) =>
                {
                    if (i != 0 && i % lineBreakAfter == 0)
                        return $"\n{string.Join(string.Empty, Enumerable.Repeat(' ', spacesInNewLine))}" + (object)(x.ToString());
                    else
                        return (object)x.ToString();
                }).ToList();
            }
            return string.Join(separator, collectionWithLineBreaks.Select(x => x.ToString()));
        }
        /// <summary>
        /// Supports following enums: ...
        /// Other types will cause an exception throw.
        /// </summary>
        //public static TEnum ToEnum<TEnum>(this string enumAsString, bool throwExceptionOnWrongParameter = true)
        //{
        //    TEnum result = default;

        //    // Do I really need to restrict enum types?
        //    Type enumType = typeof(TEnum);
        //    if (
        //        enumType != typeof(...))
        //        throw new ArgumentException($"ToEnum(..) does not support type {enumType.Name}. \nSo far it only supports the following enums: ActivationType, WeightInitType, CostType");

        //    var names = Enum.GetNames(enumType);
        //    var values = Enum.GetValues(enumType);
        //    int length = names.Length;

        //    for (int i = 0; i < length; i++)
        //    {
        //        if (names[i] == enumAsString)
        //            result = (TEnum)values.GetValue(i);

        //        if (i == length - 1 && throwExceptionOnWrongParameter)
        //            throw new ArgumentException($"{enumType.Name}.{enumAsString} does not exist.");
        //    }

        //    return result;
        //}

        #region https://stackoverflow.com/a/49407977

        public static int ReadBigInt32(this BinaryReader br)
        {
            var bytes = br.ReadBytes(sizeof(int));
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        #endregion

        // SaveAndLoad as extension methods?

    }
}
