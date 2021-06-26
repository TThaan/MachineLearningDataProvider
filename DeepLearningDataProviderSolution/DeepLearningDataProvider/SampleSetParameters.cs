using System.Collections.Generic;

namespace DeepLearningDataProvider
{
    public interface ISampleSetParameters
    {
        float InputDistortion { get; set; }
        SetName Name { get; set; }
        Dictionary<SampleType, string> Paths { get; set; }
        float TargetTolerance { get; set; }
        int DefaultTestingSamples { get; set; }
        int DefaultTrainingSamples { get; set; }
        int TestingSamples { get; set; }
        int TrainingSamples { get; set; }
        bool UseAllAvailableTestingSamples { get; set; }
        bool UseAllAvailableTrainingSamples { get; set; }
    }

    public class SampleSetParameters : ISampleSetParameters
    {
        #region ctor & fields

        public SampleSetParameters()
        {
            Paths = new Dictionary<SampleType, string>();
        }

        #endregion

        #region public

        public SetName Name { get; set; }
        public Dictionary<SampleType, string> Paths { get; set; }
        public int DefaultTestingSamples { get; set; }
        public int DefaultTrainingSamples { get; set; }
        public int TrainingSamples { get; set; }
        public int TestingSamples { get; set; }
        public bool UseAllAvailableTrainingSamples { get; set; } = true;
        public bool UseAllAvailableTestingSamples { get; set; } = true;
        public float InputDistortion { get; set; }
        public float TargetTolerance { get; set; }

        #endregion

        #region overrides

        public override string ToString()
        {
            return Name.ToString();
        }

        #endregion
    }
}
