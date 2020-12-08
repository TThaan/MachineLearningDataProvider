using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace NNet_InputProvider
{
    public abstract class SampleSet
    {
        #region ctor

        protected string defaultPath = Path.GetTempPath();
        int infoDisplayDuration = 1;//500

        /// <summary>
        /// Get the default SampleSet of the selected provider ('name').
        /// </summary>
        public SampleSet(SetName name)
        {
            Parameters = new SampleSetParameters(name);
        }
        /// <summary>
        /// Get a SampleSet with customized SampleSetParameters.
        /// </summary>
        public SampleSet(SampleSetParameters set)
        {
            Parameters = set;
        }

        #region helpers

        public async Task SetSamples()
        {
            await Task.Run(() =>
            {
                // If not all entered paths exist locally

                OnSomethingHappend($"Trying to get samples locally. (Given adresses interpreted as local path.)");
                Thread.Sleep(infoDisplayDuration);

                if (Parameters.Paths.Values.Any(x => !File.Exists(x)))
                {
                    OnSomethingHappend($"Failed to get samples locally under the given adresses. Trying default paths and file names.");
                    Thread.Sleep(infoDisplayDuration);

                    // but all files are already in the default path

                    if (Enum.GetValues(typeof(SampleType)).ToList<SampleType>()
                        .All(x => File.Exists(defaultPath + x.ToString() + Parameters.Name)))
                    {

                        OnSomethingHappend($"Found samples under default paths and file names.");
                        Thread.Sleep(infoDisplayDuration);

                        // Exchange the entered paths/url with the default ones.

                        Parameters.Paths[SampleType.TrainingLabel] = defaultPath + SampleType.TrainingLabel.ToString() + Parameters.Name;
                        Parameters.Paths[SampleType.TrainingData] = defaultPath + SampleType.TrainingData.ToString() + Parameters.Name;
                        Parameters.Paths[SampleType.TestingLabel] = defaultPath + SampleType.TestingLabel.ToString() + Parameters.Name;
                        Parameters.Paths[SampleType.TestingData] = defaultPath + SampleType.TestingData.ToString() + Parameters.Name;
                    }
                    else
                    {
                        // Otherwise try to download them

                        try
                        {
                            OnSomethingHappend($"Trying to get samples online. (Given adresses interpreted as urls.)");
                            Thread.Sleep(infoDisplayDuration);

                            Enum.GetValues(typeof(SampleType))
                                .ForEach<SampleType>(x => Parameters.Paths[x] = GetFileFromUrl(x));

                            OnSomethingHappend($"Found samples online.");
                            Thread.Sleep(infoDisplayDuration);
                        }

                        // or create them on the fly and return.

                        catch (Exception)
                        {
                            OnSomethingHappend($"Failed to get samples online. Trying to create samples.");
                            Thread.Sleep(infoDisplayDuration);

                            TrainingSamples = CreateSamples(Parameters.TrainingSamples, Parameters.InputDistortion, Parameters.TargetTolerance);
                            TestingSamples = CreateSamples(Parameters.TestingSamples, Parameters.InputDistortion, Parameters.TargetTolerance);


                            OnSomethingHappend($"Samples created by creator.");
                            Thread.Sleep(infoDisplayDuration);
                            return;
                        }
                    }
                }
                else
                {
                    OnSomethingHappend($"Found samples under the given paths and file names.");
                    Thread.Sleep(infoDisplayDuration);
                }
                // Get samples from local path.

                FileStream fs_trainLabels = new FileStream(Parameters.Paths[SampleType.TrainingLabel], FileMode.Open);
                FileStream fs_trainData = new FileStream(Parameters.Paths[SampleType.TrainingData], FileMode.Open);
                FileStream fs_testLabels = new FileStream(Parameters.Paths[SampleType.TestingLabel], FileMode.Open);
                FileStream fs_testData = new FileStream(Parameters.Paths[SampleType.TestingData], FileMode.Open);

                TrainingSamples = ConvertToSamples(fs_trainLabels, fs_trainData);
                TestingSamples = ConvertToSamples(fs_testLabels, fs_testData);

                OnSomethingHappend($"Success. Samples received.");
                Thread.Sleep(infoDisplayDuration);

            });
        }
        string GetFileFromUrl(SampleType sampleType)
        {
            string uri = Parameters.Paths[sampleType];
            string fileName = defaultPath + sampleType.ToString() + Parameters.Name;

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
        protected abstract Sample[] ConvertToSamples(FileStream fs_labels, FileStream fs_imgs);
        protected abstract Sample[] CreateSamples(int samples, float inputDistortion, float targetTolerance);

        #endregion

        #endregion

        #region public

        public SampleSetParameters Parameters { get; protected set; }        
        public Sample[] TrainingSamples { get; protected set; }
        public Sample[] TestingSamples { get; protected set; }

        #endregion

        // ... not needed?
        protected string DownloadFileAndGetPath(string uri)
        {
            string path = Path.GetTempPath() + @"trainDataLabels.gz";
            byte[] labelsArray = new WebClient().DownloadData(uri);

            using (var memStream = new MemoryStream(labelsArray))
            {
                using (var zipStream = new GZipStream(memStream, CompressionMode.Decompress))
                {
                    byte[] buffer = new byte[zipStream.Length];
                    zipStream.Read(buffer, 0, buffer.Length);

                    using (var fileStream = File.Create(path))
                    {
                        fileStream.Write(buffer, 0, buffer.Length);
                        // or without saving to disk..

                    }
                }
            }

            return path;
        }

        #region Events

        public delegate void SomethingHappendEventHandler(string whatHappend);
        public event SomethingHappendEventHandler SomethingHappend;
        void OnSomethingHappend(string whatHappend)
        {
            SomethingHappend?.Invoke(whatHappend);
        }

        //public delegate Task<bool> PausedEventHandler(string pauseInfo);
        //public event PausedEventHandler Paused;
        //async Task<bool> OnPausedAsync(string pauseInfo)
        //{
        //    if (Paused == null)
        //        return true;
        //    return await Paused.Invoke(pauseInfo);
        //}

        #endregion
    }
}
