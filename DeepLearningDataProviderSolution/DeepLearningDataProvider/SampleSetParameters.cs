using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace DeepLearningDataProvider
{
    public interface ISampleSetParameters
    {
        float InputDistortion { get; set; }
        SetName Name { get; set; }
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
            Paths = new Dictionary<SampleType, string>();
        }

        #endregion

        #region public

        [JsonConverter(typeof(StringEnumConverter))]
        public SetName Name { get; set; }
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
