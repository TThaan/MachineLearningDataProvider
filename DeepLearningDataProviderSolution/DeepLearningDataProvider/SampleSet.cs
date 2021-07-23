using Microsoft.ML;

namespace DeepLearningDataProvider
{
    public interface ISampleSet
    {
        //ISampleSetParameters Parameters { get; set; }
        //float[][] TestingSamples { get; set; }
        //float[][] TrainingSamples { get; set; }
        //float[][] TestingTargets { get; set; }
        //float[][] TrainingTargets { get; set; }
        Sample[] TestSet { get; set; }
        Sample[] TrainSet { get; set; }
        //Sample[] TestTargets { get; set; }
        //Sample[] TrainTargets { get; set; }
    }

    //[Serializable]
    //[JsonObject()]
    public class SampleSet : ISampleSet
    {
        //public float[][] TestingSamples { get; set; }
        //public float[][] TrainingSamples { get; set; }
        //public float[][] TestingTargets { get; set; }
        //public float[][] TrainingTargets { get; set; }
        public Sample[] TestSet { get; set; }
        public Sample[] TrainSet { get; set; }
        //public Sample[] TestTargets { get; set; }
        //public Sample[] TrainTargets { get; set; }
    }
}
