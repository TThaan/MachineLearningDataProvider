namespace DeepLearningDataProvider
{
    public interface ISampleSet
    {
        SampleSetParameters Parameters { get; set; }
        Sample[] TestingSamples { get; set; }
        Sample[] TrainingSamples { get; set; }
    }

    // [Serializable]
    public class SampleSet : ISampleSet
    {
        public SampleSetParameters Parameters { get; set; }
        public Sample[] TrainingSamples { get; set; }
        public Sample[] TestingSamples { get; set; }
    }
}
