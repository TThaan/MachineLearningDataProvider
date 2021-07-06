using DeepLearningDataProvider.Builders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DeepLearningDataProvider
{
    public class Initializer
    {
        #region fields & ctor

        private PathBuilder pathBuilder;
        private SampleSetParameterBuilder parameterBuilder;
        private ISampleSet sampleSet;

        public Initializer()
        {
            pathBuilder = new PathBuilder(OnDataProviderChanged);                                 // via DC?
            parameterBuilder = new SampleSetParameterBuilder(pathBuilder, OnDataProviderChanged); // via DC?
        }

        #endregion

        #region properties

        public PathBuilder PathBuilder
        {
            get
            {
                if (pathBuilder == null)
                    OnDataProviderChanged("Paths are null");
                return pathBuilder;
            }
        }
        public SampleSetParameterBuilder ParameterBuilder
        {
            get
            {
                if (parameterBuilder == null)
                    OnDataProviderChanged("ParameterBuilder are null");
                return parameterBuilder;
            }
        }
        public ISampleSet SampleSet
        {
            get
            {
                if (sampleSet == null)
                    OnDataProviderChanged("SampleSet is null");
                return sampleSet;
            }
            set { sampleSet = value; }
        }

        #endregion

        #region methods

        public async Task<bool> CreateSampleSetAsync()
        {
            if (ParameterBuilder.Parameters == null)
            {
                OnDataProviderChanged("No sample set parameters are set.");
                return false;
            }

            try
            {
                var sampleSetSteward = new SampleSetSteward();

                OnDataProviderChanged("Creating samples, please wait...");
                SampleSet = await sampleSetSteward.CreateCustomSampleSetAsync(ParameterBuilder.Parameters);
                SampleSet.Parameters = ParameterBuilder.Parameters;
                OnDataProviderChanged("Successfully created samples.");
                return true;
            }
            catch (Exception e) { OnDataProviderChanged(e.Message); return false; }
        }
        public async Task<bool> LoadSampleSetAsync()
        {
            try
            {
                OnDataProviderChanged("Loading samples from file, please wait...");
                var jsonString = await File.ReadAllTextAsync(PathBuilder.SampleSet);

                dynamic dynamicSampleSet = JObject.Parse(jsonString);
                SampleSetParameters parameters = ((JObject)dynamicSampleSet.Parameters).ToObject<SampleSetParameters>();
                Sample[] testingSamples = ((JArray)dynamicSampleSet.TestingSamples).ToObject<Sample[]>();
                Sample[] trainingSamples = ((JArray)dynamicSampleSet.TrainingSamples).ToObject<Sample[]>();
                SampleSet = new SampleSet
                {
                    Parameters = parameters,
                    TestingSamples = testingSamples,
                    TrainingSamples = trainingSamples
                };
                Sample.Tolerance = parameters.TargetTolerance;
                OnDataProviderChanged("Successfully loaded samples.");
                return true;
            }
            catch (Exception e) { OnDataProviderChanged(e.Message); return false; }
        }
        public async Task<bool> SaveSampleSetAsync()
        {
            try
            {
                OnDataProviderChanged("Saving sample set, please wait...");

                var jsonString = JsonConvert.SerializeObject(SampleSet, Formatting.Indented);
                await File.AppendAllTextAsync(PathBuilder.SampleSet, jsonString);

                OnDataProviderChanged("Successfully saved sample set.");
                return true;
            }
            catch (Exception e) { OnDataProviderChanged(e.Message); return false; }
        }

        #endregion

        #region DataProviderEventHandler

        public event DataProviderChangedEventHandler DataProviderChanged;
        void OnDataProviderChanged(string info)
        {
            DataProviderChanged?.Invoke(this, new DataProviderChangedEventArgs(info));
        }

        #endregion
    }
}
