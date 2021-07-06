using DeepLearningDataProvider.JsonConverters;
using Newtonsoft.Json;
using System;

namespace DeepLearningDataProvider
{
    public interface ISampleSet
    {
        ISampleSetParameters Parameters { get; set; }
        Sample[] TestingSamples { get; set; }
        Sample[] TrainingSamples { get; set; }
    }

    //[Serializable]
    //[JsonObject()]
    public class SampleSet : ISampleSet
    {
        public ISampleSetParameters Parameters { get; set; } // ISampleSetParameters?
        //[JsonConverter(typeof(JsonConverter_Array<Sample>))]
        //[JsonProperty(ItemConverterType = typeof(GenericJsonConverter<Sample>))]
        public Sample[] TrainingSamples { get; set; }   // ISample?
        //[JsonConverter(typeof(JsonConverter_Array<Sample>))]
        //[JsonProperty(ItemConverterType = typeof(GenericJsonConverter<Sample>))]
        public Sample[] TestingSamples { get; set; }    // ISample?
    }
}
