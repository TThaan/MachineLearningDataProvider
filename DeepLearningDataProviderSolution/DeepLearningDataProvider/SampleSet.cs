using DeepLearningDataProvider.JsonConverters;
using Newtonsoft.Json;
using System;

namespace DeepLearningDataProvider
{
    public interface ISampleSet
    {
        SampleSetParameters Parameters { get; set; }
        Sample[] TestingSamples { get; set; }
        Sample[] TrainingSamples { get; set; }
    }

    //[Serializable]
    //[JsonObject()]
    public class SampleSet : ISampleSet
    {
        public SampleSetParameters Parameters { get; set; }
        //[JsonConverter(typeof(JsonConverter_Array<Sample>))]
        [JsonProperty(ItemConverterType = typeof(GenericJsonConverter<Sample>))]
        public Sample[] TrainingSamples { get; set; }
        //[JsonConverter(typeof(JsonConverter_Array<Sample>))]
        //[JsonProperty(ItemConverterType = typeof(GenericJsonConverter<Sample>))]
        public Sample[] TestingSamples { get; set; }
    }
}
