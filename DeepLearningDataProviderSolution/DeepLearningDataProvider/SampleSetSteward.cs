using DeepLearningDataProvider.Factories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace DeepLearningDataProvider
{
    /// <summary>
    /// Steward = Factory + Store
    /// </summary>
    public interface ISampleSetSteward
    {
        ISampleSet SampleSet { get; }
        // bool IsSampleSetNull { get; }
        Dictionary<SetName, ISampleSetParameters> DefaultSampleSetParameters { get; }
        IEnumerable<SampleType> Types { get; }
        Task<ISampleSet> CreateSampleSetAsync(ISampleSetParameters sampleSetParameters);
        Task<ISampleSet> CreateDefaultSampleSetAsync(SetName setName);
        string Status { get; }
        PropertyChangedEventHandler[] EventHandlers { get; set; }
    }

    /// <summary>
    /// Steward = Factory + Store
    /// </summary>
    public class SampleSetSteward : ISampleSetSteward
    {
        #region fields & ctor

        Dictionary<SetName, ISampleSetParameters> defaultSampleSetParameters;
        IEnumerable<SampleType> types;
        SampleSetFactoryBase factory;

        #endregion

        #region public

        public ISampleSet SampleSet { get; private set; }
        public Dictionary<SetName, ISampleSetParameters> DefaultSampleSetParameters => 
            defaultSampleSetParameters ?? (defaultSampleSetParameters = GetDefaultSampleSetParameters());
        public IEnumerable<SampleType> Types => types ?? (types = GetTypes());

        /// <summary>
        /// Get a SampleSet with customized parameters.
        /// </summary>
        public async Task<ISampleSet> CreateSampleSetAsync(ISampleSetParameters sampleSetParameters)
        {
            factory = GetDedicatedFactory(sampleSetParameters.Name);
            return SampleSet = await factory.CreateSampleSetAsync(sampleSetParameters);
        }
        /// <summary>
        /// Get the default SampleSet for a given SetName
        /// </summary>
        public async Task<ISampleSet> CreateDefaultSampleSetAsync(SetName setName)
        {
            factory = GetDedicatedFactory(setName);
            return SampleSet = await factory.CreateDefaultSampleSetAsync(setName);
        }
        public PropertyChangedEventHandler[] EventHandlers { get; set; }
        public string Status => factory?.Status;

        #region helpers

        private SampleSetFactoryBase GetDedicatedFactory(SetName setName)
        {
            SampleSetFactoryBase result;
            switch (setName)
            {
                case SetName.FourPixelCamera:
                    result = new FourPixCamSampleSetFactory();
                    break;
                case SetName.MNIST:
                    result = new MNISTSampleSetFactory();
                    break;
                default:
                    throw new ArgumentException($"Couldn't find a fitting SampleSet to the given SetName {setName}.");
            }
            EventHandlers.ForEach<PropertyChangedEventHandler>(x => result.PropertyChanged += x);
            return result;
        }
        private Dictionary<SetName, ISampleSetParameters> GetDefaultSampleSetParameters()
        {
            return new Dictionary<SetName, ISampleSetParameters>
            {
                [SetName.FourPixelCamera] = new SampleSetParameters()
                {
                    Name = SetName.FourPixelCamera,
                    Paths = new Dictionary<SampleType, string>
                    {
                        [SampleType.TrainingLabel] = "To be created..",
                        [SampleType.TrainingData] = "To be created..",
                        [SampleType.TestingLabel] = "To be created..",
                        [SampleType.TestingData] = "To be created.."
                    },
                    DefaultTrainingSamples = 1000,
                    DefaultTestingSamples = 16,
                    TrainingSamples = 1000,
                    TestingSamples = 16,
                    InputDistortion = 0.2f,
                    TargetTolerance = .3f
                },
                [SetName.MNIST] = new SampleSetParameters()
                {
                    Name = SetName.MNIST,
                    Paths = new Dictionary<SampleType, string>
                    {
                        [SampleType.TrainingLabel] = "http://yann.lecun.com/exdb/mnist/train-labels-idx1-ubyte.gz",
                        [SampleType.TrainingData] = "http://yann.lecun.com/exdb/mnist/train-images-idx3-ubyte.gz",
                        [SampleType.TestingLabel] = "http://yann.lecun.com/exdb/mnist/t10k-labels-idx1-ubyte.gz",
                        [SampleType.TestingData] = "http://yann.lecun.com/exdb/mnist/t10k-images-idx3-ubyte.gz"
                    },
                    DefaultTrainingSamples = 60000,
                    DefaultTestingSamples = 10000,
                    TrainingSamples = 60000,
                    TestingSamples = 10000
                }
            };
        }
        private IEnumerable<SampleType> GetTypes()
        {
            return Enum.GetValues(typeof(SampleType)).ToList<SampleType>();
        }

        #endregion

        #endregion
    }
}
