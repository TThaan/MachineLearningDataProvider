using System;
using System.Collections.Generic;

namespace NNet_InputProvider
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

        public static Dictionary<SetName, SampleSetParameters> Templates { get; private set; } = GetTemplates();
        public static IEnumerable<SampleType> Types { get; private set; } = GetTypes();

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

        #region helpers

        static Dictionary<SetName, SampleSetParameters> GetTemplates()
        {
            return new Dictionary<SetName, SampleSetParameters>
            {
                [SetName.FourPixelCamera] = new SampleSetParameters(SetName.FourPixelCamera)
                {
                    Paths = new Dictionary<SampleType, string>
                    {
                        [SampleType.TrainingLabel] = "To be created..",
                        [SampleType.TrainingData] = "To be created..",
                        [SampleType.TestingLabel] = "To be created..",
                        [SampleType.TestingData] = "To be created.."
                    },
                    TrainingSamples = 250,
                    TestingSamples = 16,
                    InputDistortion = .2f,
                    TargetTolerance = .3f
                },
                [SetName.MNIST] = new SampleSetParameters(SetName.MNIST)
                {
                    Paths = new Dictionary<SampleType, string>
                    {
                        [SampleType.TrainingLabel] = "http://yann.lecun.com/exdb/mnist/train-images-idx3-ubyte.gz",
                        [SampleType.TrainingData] = "http://yann.lecun.com/exdb/mnist/train-labels-idx1-ubyte.gz",
                        [SampleType.TestingLabel] = "http://yann.lecun.com/exdb/mnist/t10k-images-idx3-ubyte.gz",
                        [SampleType.TestingData] = "http://yann.lecun.com/exdb/mnist/t10k-labels-idx1-ubyte.gz"
                    }
                }
            };
        }
        static IEnumerable<SampleType> GetTypes()
        {
            return Enum.GetValues(typeof(SampleType)).ToList<SampleType>();
        }

        #endregion
    }
}
