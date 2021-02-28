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
        string Status { get; }
        ISampleSet SampleSet { get; }
        Dictionary<SetName, ISampleSetParameters> Templates { get; }
        IEnumerable<SampleType> Types { get; }
        Task<ISampleSet> CreateSampleSetAsync(ISampleSetParameters sampleSetParameters);
        Task<ISampleSet> CreateDefaultSampleSetAsync(SetName setName);
    }

    /// <summary>
    /// Steward = Factory + Store
    /// </summary>
    public class SampleSetSteward : ISampleSetSteward
    {
        #region fields & ctor

        private readonly PropertyChangedEventHandler[] _eventHandlers;
        Dictionary<SetName, ISampleSetParameters> templates;
        IEnumerable<SampleType> types;

        public SampleSetSteward(params PropertyChangedEventHandler[] eventHandlers)
        {
            _eventHandlers = eventHandlers;
        }

        #endregion

        #region public

        public string Status { get; private set; }
        public ISampleSet SampleSet { get; private set; }
        public Dictionary<SetName, ISampleSetParameters> Templates => templates ?? (templates = GetTemplates());
        public IEnumerable<SampleType> Types => types ?? (types = GetTypes());

        /// <summary>
        /// Get a SampleSet with customized parameters.
        /// </summary>
        public async Task<ISampleSet> CreateSampleSetAsync(ISampleSetParameters sampleSetParameters)
        {
            SampleSetFactoryBase factory = GetDedicatedFactory(sampleSetParameters.Name);
            return SampleSet = await factory.CreateSampleSetAsync(sampleSetParameters);
        }
        /// <summary>
        /// Get the default SampleSet for a given SetName
        /// </summary>
        public async Task<ISampleSet> CreateDefaultSampleSetAsync(SetName setName)
        {
            SampleSetFactoryBase factory = GetDedicatedFactory(setName);
            return SampleSet = await factory.CreateDefaultSampleSetAsync(setName);
        }

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
            Status = result.Status;
            _eventHandlers.ForEach<PropertyChangedEventHandler>(x => result.PropertyChanged += x);
            return result;
        }
        private Dictionary<SetName, ISampleSetParameters> GetTemplates()
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
