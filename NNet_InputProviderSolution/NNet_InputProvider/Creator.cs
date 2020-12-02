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
            switch (sampleSetParameters.Name)
            {
                case SetName.FourPixelCamera:
                    return new FourPixCamSampleSet(sampleSetParameters);
                case SetName.MNIST:
                    return new MNISTSampleSet(sampleSetParameters);
                default:
                    throw new ArgumentException($"Couldn't find a fitting SampleSet to given SetName {sampleSetParameters.Name}.");
            }
        }
        public static SampleSet GetSampleSet(SetName setName)
        {
            switch (setName)
            {
                case SetName.FourPixelCamera:
                    return new FourPixCamSampleSet(setName);
                case SetName.MNIST:
                    return new MNISTSampleSet(setName);
                default:
                    throw new ArgumentException($"Couldn't find a fitting SampleSet to given SetName {setName}.");
            }
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
