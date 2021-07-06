using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DeepLearningDataProvider.Builders
{
    public class SampleSetParameterBuilder
    {
        #region fields & ctor

        private readonly Action<string> _onDataProviderChanged;
        private readonly PathBuilder _pathBuilder;
        private ISampleSetParameters parameters;

        public SampleSetParameterBuilder(PathBuilder paths, Action<string> onDataProviderChanged)
        {
            _pathBuilder = paths;
            _onDataProviderChanged = onDataProviderChanged;
        }

        #endregion

        public IEnumerable<SetName> DefaultParameters => new SampleSetSteward().DefaultParameters.Keys;
        public ISampleSetParameters Parameters
        {
            get
            {
                if (parameters == null)
                    _onDataProviderChanged("SampleSetParameters are null");
                return parameters;
            }
            set
            {
                parameters = value;
            }
        }

        #region methods

        public void ChangeParameter(string parameterName, string parameterValue)
        {
            try
            {
                switch (parameterName)
                {
                    case nameof(Parameters.Name):
                        SetSampleSetName(parameterValue.ToEnum<SetName>());
                        break;
                    case nameof(Parameters.TestingSamples):
                        SetAmountOfTestingSamples(int.Parse(parameterValue));
                        break;
                    case nameof(Parameters.TrainingSamples):
                        SetAmountOfTrainingSamples(int.Parse(parameterValue));
                        break;
                    case nameof(Parameters.InputDistortion):
                        SetInputDistortion(int.Parse(parameterValue));
                        break;
                    case nameof(Parameters.TargetTolerance):
                        SetTargetTolerance(float.Parse(parameterValue));
                        break;
                }
            }
            catch (Exception e) { _onDataProviderChanged(e.Message); }
        }

        public bool SetSampleSetName(SetName name)
        {
            try
            {
                Parameters.Name = name;
            }
            catch (Exception e) { _onDataProviderChanged(e.Message); return false; }

            _onDataProviderChanged($"{Parameters}.{Parameters.Name} has been set to {Parameters.Name}.");
            return true;
        }
        public bool SetAmountOfTestingSamples(int testingSamples)
        {
            try
            {
                Parameters.TestingSamples = testingSamples;
            }
            catch (Exception e) { _onDataProviderChanged(e.Message); return false; }

            _onDataProviderChanged($"{Parameters}.{Parameters.TestingSamples} has been set to {Parameters.TestingSamples}.");
            return true;
        }
        public bool SetAmountOfTrainingSamples(int trainingSamples)
        {
            try
            {
                Parameters.TrainingSamples = trainingSamples;
            }
            catch (Exception e) { _onDataProviderChanged(e.Message); return false; }

            _onDataProviderChanged($"{Parameters}.{Parameters.TrainingSamples} has been set to {Parameters.TrainingSamples}.");
            return true;
        }
        public bool SetInputDistortion(int targetTolerance)
        {
            try
            {
                Parameters.InputDistortion = targetTolerance;
            }
            catch (Exception e) { _onDataProviderChanged(e.Message); return false; }

            _onDataProviderChanged($"{Parameters}.{Parameters.InputDistortion} of samples has been set to {Parameters.InputDistortion}.");
            return true;
        }
        public bool SetTargetTolerance(float targetTolerance)
        {
            try
            {
                Parameters.TargetTolerance = targetTolerance;
            }
            catch (Exception e) { _onDataProviderChanged(e.Message); return false; }

            _onDataProviderChanged($"{Parameters}.{Parameters.TargetTolerance} of samples has been set to {Parameters.TargetTolerance}.");
            return true;
        }
        public bool UseAllAvailableTestingSamples()
        {
            try
            {
                Parameters.TestingSamples = Parameters.AllTestingSamples;
            }
            catch (Exception e) { _onDataProviderChanged(e.Message); return false; }

            _onDataProviderChanged($"{Parameters}.{Parameters.TestingSamples} has been set to {Parameters.AllTestingSamples}.");
            return true;
        }
        public bool UseAllAvailableTrainingSamples()
        {
            try
            {
                Parameters.TestingSamples = Parameters.AllTestingSamples;
            }
            catch (Exception e) { _onDataProviderChanged(e.Message); return false; }

            _onDataProviderChanged($"{Parameters}.{Parameters.TestingSamples} has been set to {Parameters.AllTrainingSamples}.");
            return true;
        }
        public bool SetSamplePaths()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region methods: Create, Load & Save

        public bool CreateSampleSetParameters(string templateName)
        {
            if (templateName == null)
                templateName = "FourPixelCamera";

            SetName name = SetName.Custom;

            switch (templateName)
            {
                case "FourPixelCamera":
                    name = SetName.FourPixelCamera;
                    break;
                case "MNIST":
                    name = SetName.MNIST;
                    break;
            }
            if (name == SetName.Custom)
            {
                _onDataProviderChanged($"Template name {templateName} is unavailable.");
                return false;
            }

            Parameters = new SampleSetSteward().DefaultParameters[name];
            _onDataProviderChanged("Sample set parameters created.");
            return true;
        }
        public async Task<bool> LoadSampleSetParametersAsync()
        {
            if (_pathBuilder.SampleSetParameters == default)
            {
                _onDataProviderChanged("No path to sample set parameters is set.");
                return false;
            }

            try
            {
                _onDataProviderChanged("Loading sample set parameters from file, please wait...");
                var jsonString = await File.ReadAllTextAsync(_pathBuilder.SampleSetParameters);
                Parameters = JsonConvert.DeserializeObject<SampleSetParameters>(jsonString);
                _onDataProviderChanged("Successfully loaded sample set parameters.");
                return true;
            }
            catch (Exception e) { _onDataProviderChanged(e.Message); return false; }
        }
        public async Task<bool> SaveSampleSetParametersAsync()
        {
            try
            {
                _onDataProviderChanged("Saving sample set parameters, please wait...");

                var jsonString = JsonConvert.SerializeObject(Parameters, Formatting.Indented);
                await File.AppendAllTextAsync(_pathBuilder.SampleSetParameters, jsonString);

                _onDataProviderChanged("Successfully saved sample set parameters.");
                return true;
            }
            catch (Exception e) { _onDataProviderChanged(e.Message); return false; }
        }

        #endregion
    }
}
