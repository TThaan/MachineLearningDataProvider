using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;

namespace NNet_InputProvider
{
    public abstract class BaseDataFactory
    {
        #region ctor

        /// <summary>
        /// Get the default SampleSet of the selected provider ('name').
        /// </summary>
        public BaseDataFactory(SetName name)
        {
            Set = new SampleSetParameters(name);
            SetDefaultValues();
        }
        /// <summary>
        /// Get a SampleSet with customized SampleSetParameters.
        /// </summary>
        public BaseDataFactory(SampleSetParameters set)
        {
            Set = set;
            SetDefaultValues();
        }

        #region helpers

        void SetDefaultValues()
        {
            GetSamples();
            if (TrainingSamples?.Length! > 0 || TestingSamples?.Length! > 0)
            {
                TrainingSamples = CreateSamples(Set.TrainingSamples, Set.InputDistortion, Set.TargetTolerance);
                TestingSamples = CreateSamples(Set.TestingSamples, Set.InputDistortion, Set.TargetTolerance);
            }
        }
        void GetSamples()
        {
            // If not all paths exist locally

            if (Set.Paths.Values.Any(x => !File.Exists(x)))
            {
                // try them as urls and store them locally.

                try
                {
                    Enum.GetValues(typeof(SampleType))
                        .ForEach<SampleType>(x => Set.Paths[x] = GetFileFromUrl(x));
                }
                catch (Exception)
                {
                    throw;
                }
            }

            // Get samples from local path.

            FileStream fs_trainLabels = new FileStream(Set.Paths[SampleType.TestingData], FileMode.Open);
            FileStream fs_trainData = new FileStream(Set.Paths[SampleType.TestingData], FileMode.Open);
            FileStream fs_testLabels = new FileStream(Set.Paths[SampleType.TestingData], FileMode.Open);
            FileStream fs_testData = new FileStream(Set.Paths[SampleType.TestingData], FileMode.Open);

            TrainingSamples = GetSamplesFromStream(fs_trainLabels, fs_trainData);
            TestingSamples = GetSamplesFromStream(fs_testLabels, fs_testData);
        }
        string GetFileFromUrl(SampleType sampleType)
        {
            string uri = Set.Paths[sampleType];
            string localPath = Path.GetTempPath() + sampleType.ToString();

            byte[] labelsArray = new WebClient().DownloadData(uri);

            using (var memStream = new MemoryStream(labelsArray))
            {
                using (var zipStream = new GZipStream(memStream, CompressionMode.Decompress))
                {
                    byte[] buffer = new byte[0x10000];

                    using (var fileStream = File.Create(localPath))
                    {
                        while (zipStream.Read(buffer, 0, buffer.Length) > 0)
                        {
                            fileStream.Write(buffer, 0, buffer.Length);
                        }
                        return localPath;
                    }
                }
            }
        }
        protected abstract Sample[] GetSamplesFromStream(FileStream fs_labels, FileStream fs_imgs);
        protected abstract Sample[] CreateSamples(int samples, float inputDistortion, float targetTolerance);

        #endregion

        #endregion

        #region public

        public SampleSetParameters Set { get; protected set; }        
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
                        // or without saving to disk

                    }
                }
            }

            return path;
        }
    }
}
