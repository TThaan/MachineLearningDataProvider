using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace DeepLearningDataProvider
{
    public interface ISampleSetParameters
    {
        float InputDistortion { get; set; }
        string Name { get; set; }
        Dictionary<SampleType, string> Paths { get; set; }
        float TargetTolerance { get; set; }
        int TestingSamples { get; set; }
        int TrainingSamples { get; set; }
        int AllTestingSamples { get; set; }
        int AllTrainingSamples { get; set; }
    }

    public class SampleSetParameters : ISampleSetParameters
    {
        #region ctor & fields

        public SampleSetParameters()
        {
            Name = "DefaultParameters";
            Paths = new Dictionary<SampleType, string>
            {
                [SampleType.TrainingLabel] = "To be created..",
                [SampleType.TrainingData] = "To be created..",
                [SampleType.TestingLabel] = "To be created..",
                [SampleType.TestingData] = "To be created.."
            };
            AllTrainingSamples = 1000;
            AllTestingSamples = 16;
            TrainingSamples = 1000;
            TestingSamples = 16;
            InputDistortion = .2f;
            TargetTolerance = .3f;
        }

        #endregion

        #region public

        //[JsonConverter(typeof(StringEnumConverter))]
        public string Name { get; set; }
        public Dictionary<SampleType, string> Paths { get; set; }
        public int AllTestingSamples { get; set; }
        public int AllTrainingSamples { get; set; }
        public int TrainingSamples { get; set; }
        public int TestingSamples { get; set; }
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
