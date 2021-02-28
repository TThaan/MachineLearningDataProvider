using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace DeepLearningDataProvider.Factories
{
    public interface ISampleSetFactory
    {
        event PropertyChangedEventHandler PropertyChanged;
    }

    public abstract class SampleSetFactoryBase : INotifyPropertyChanged, ISampleSetFactory
    {
        #region fields

        protected Random rnd;

        private string status, defaultPath = Path.GetTempPath();
        private int statusDisplayDuration = 1000; // variable

        #endregion

        #region internal members

        internal string Status
        {
            get { return status; }
            set
            {
                if (status != value)
                {
                    status = value;
                    OnPropertyChanged();
                };
            }
        }
        // Refactor..
        internal async Task<ISampleSet> CreateSampleSetAsync(ISampleSetParameters parameters)
        {
            return await Task.Run(() =>
            {
                SampleSet result = new SampleSet();

                // If not all entered paths exist locally

                Status = $"Trying to get samples locally. (Given adresses interpreted as local path.)";
                Thread.Sleep(statusDisplayDuration);

                if (parameters.Paths == null || parameters.Paths.Count == 0 ||
                parameters.Paths.Values.Any(x => !File.Exists(x)))
                {
                    Status = $"Failed to get samples locally under the given adresses. Trying default paths and file names.";
                    Thread.Sleep(statusDisplayDuration);

                    // but all files are already in the default path

                    if (Enum.GetValues(typeof(SampleType)).ToList<SampleType>()
                        .All(x => File.Exists(defaultPath + x.ToString() + parameters.Name)))
                    {

                        Status = $"Found samples under default paths and file names.";
                        Thread.Sleep(statusDisplayDuration);

                        // Exchange the entered paths/url with the default ones.

                        parameters.Paths[SampleType.TrainingLabel] = defaultPath + SampleType.TrainingLabel.ToString() + parameters.Name;
                        parameters.Paths[SampleType.TrainingData] = defaultPath + SampleType.TrainingData.ToString() + parameters.Name;
                        parameters.Paths[SampleType.TestingLabel] = defaultPath + SampleType.TestingLabel.ToString() + parameters.Name;
                        parameters.Paths[SampleType.TestingData] = defaultPath + SampleType.TestingData.ToString() + parameters.Name;
                    }
                    else
                    {
                        // Otherwise try to download them

                        try
                        {
                            Status = $"Trying to get samples online. (Given adresses interpreted as urls.)";
                            Thread.Sleep(statusDisplayDuration);

                            Enum.GetValues(typeof(SampleType))
                                .ForEach<SampleType>(x => parameters.Paths[x] = GetFileFromUrl(parameters, x));

                            Status = $"Found samples online.";
                            Thread.Sleep(statusDisplayDuration);
                        }

                        // or create them on the fly and return.

                        catch (Exception)
                        {
                            Status = $"Failed to get samples online. Trying to create samples.";
                            Thread.Sleep(statusDisplayDuration);

                            result.TrainingSamples = CreateSamples(parameters.TrainingSamples, parameters.InputDistortion, parameters.TargetTolerance);
                            result.TestingSamples = CreateSamples(parameters.TestingSamples, parameters.InputDistortion, parameters.TargetTolerance);


                            Status = $"Samples created by creator.";
                            Thread.Sleep(statusDisplayDuration);
                            return result;
                        }
                    }
                }
                else
                {
                    Status = $"Found samples under the given paths and file names.";
                    Thread.Sleep(statusDisplayDuration);
                }
                // Get samples from local path.

                FileStream fs_trainLabels = new FileStream(parameters.Paths[SampleType.TrainingLabel], FileMode.Open);
                FileStream fs_trainData = new FileStream(parameters.Paths[SampleType.TrainingData], FileMode.Open);
                FileStream fs_testLabels = new FileStream(parameters.Paths[SampleType.TestingLabel], FileMode.Open);
                FileStream fs_testData = new FileStream(parameters.Paths[SampleType.TestingData], FileMode.Open);

                result.TrainingSamples = ConvertFilesToSamples(fs_trainLabels, fs_trainData);
                result.TestingSamples = ConvertFilesToSamples(fs_testLabels, fs_testData);

                Status = $"Success. Samples received.";
                Thread.Sleep(statusDisplayDuration);

                return result;
            });
        }
        internal abstract Task<ISampleSet> CreateDefaultSampleSetAsync(SetName setName);

        #region helpers

        private string GetFileFromUrl(ISampleSetParameters sampleSetParameters, SampleType sampleType)
        {
            string uri = sampleSetParameters.Paths[sampleType];
            string fileName = defaultPath + sampleType.ToString() + sampleSetParameters.Name;

            byte[] labelsArray = new WebClient().DownloadData(uri);

            using (var memStream = new MemoryStream(labelsArray))
            {
                using (var zipStream = new GZipStream(memStream, CompressionMode.Decompress))
                {
                    byte[] buffer = new byte[0x10000];

                    using (var fileStream = File.Create(fileName))
                    {
                        while (zipStream.Read(buffer, 0, buffer.Length) > 0)
                        {
                            fileStream.Write(buffer, 0, buffer.Length);
                        }
                        return fileName;
                    }
                }
            }
        }
        protected abstract Sample[] ConvertFilesToSamples(FileStream fs_labels, FileStream fs_imgs);
        protected abstract Sample[] CreateSamples(int samplesCount, float inputDistortion, float targetTolerance);

        #endregion

        #endregion

        #region INPC

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
