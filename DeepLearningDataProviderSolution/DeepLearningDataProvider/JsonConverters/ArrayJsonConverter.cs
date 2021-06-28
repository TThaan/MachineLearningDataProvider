using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace DeepLearningDataProvider.JsonConverters
{
    public class JsonConverter_Array<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //serializer.TypeNameHandling = TypeNameHandling.All;
            JArray arr = (JArray)serializer.Deserialize(reader);
            T[] result = new T[arr.Count];
            for (int i = 0; i < arr.Count; i++)
            {
                var v = arr[i].ToObject<T>();
                result[i] = v;
            }
            //arr.CopyTo(result, 0);
            //var hm = arr.ToObject(typeof(List<T>));
            
            //T[] result = arr.ToObject(typeof(T[])) as T[];
            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //serializer.TypeNameHandling = TypeNameHandling.All;
            serializer.Serialize(writer, value);
        }
    }
}
