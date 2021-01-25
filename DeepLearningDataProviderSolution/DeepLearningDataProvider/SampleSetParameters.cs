using System.Collections.Generic;

namespace DeepLearningDataProvider
{
    public class SampleSetParameters
    {
        #region ctor & fields

        public SampleSetParameters(SetName name)
        {
            Name = name;
            Paths = new Dictionary<SampleType, string>();
        }

        #endregion

        #region public

        public SetName Name { get; set; }
        public Dictionary<SampleType, string> Paths { get; set; }
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
