using NNet_InputProvider.FourPixCam;
using NNet_InputProvider.MNIST;
using System;
using System.Collections.Generic;

namespace NNet_InputProvider
{
    public static class Creator
    {
        #region public

        public static Dictionary<SetName, SampleSetParameters> Templates { get; private set; } = GetTemplates();
        public static IEnumerable<SampleType> Types { get; private set; } = GetTypes();

        public static SampleSet GetSampleSet(SampleSetParameters sampleSetParameters)
        {
            SampleSet result;

            switch (sampleSetParameters.Name)
            {
                case SetName.FourPixelCamera:
                    result = new FourPixCamSampleSet(sampleSetParameters);
                    break;
                case SetName.MNIST:
                    result = new MNISTSampleSet(sampleSetParameters);
                    break;
                default:
                    throw new ArgumentException($"Couldn't find a fitting SampleSet to given SetName {sampleSetParameters.Name}.");
            }
            return result;
        }
        public static SampleSet GetSampleSet(SetName setName)
        {
            SampleSet result;

            switch (setName)
            {
                case SetName.FourPixelCamera:
                    result = new FourPixCamSampleSet(setName);
                    break;
                case SetName.MNIST:
                    result = new MNISTSampleSet(setName);
                    break;
                default:
                    throw new ArgumentException($"Couldn't find a fitting SampleSet to given SetName {setName}.");
            }
            return result;
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
                    TrainingSamples = 1000,
                    TestingSamples = 16,
                    InputDistortion = 0.2f,
                    TargetTolerance = .3f
                },
                [SetName.MNIST] = new SampleSetParameters(SetName.MNIST)
                {
                    Paths = new Dictionary<SampleType, string>
                    {
                        [SampleType.TrainingLabel] = "http://yann.lecun.com/exdb/mnist/train-labels-idx1-ubyte.gz",
                        [SampleType.TrainingData] = "http://yann.lecun.com/exdb/mnist/train-images-idx3-ubyte.gz",
                        [SampleType.TestingLabel] = "http://yann.lecun.com/exdb/mnist/t10k-labels-idx1-ubyte.gz",
                        [SampleType.TestingData] = "http://yann.lecun.com/exdb/mnist/t10k-images-idx3-ubyte.gz"
                    },
                    TrainingSamples = 60000,
                    TestingSamples = 10000
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
