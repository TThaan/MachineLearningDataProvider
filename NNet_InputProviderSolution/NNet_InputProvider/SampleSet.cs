using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;

namespace NNet_InputProvider
{
    public abstract class SampleSet
    {
        #region ctor

        /// <summary>
        /// Get the default SampleSet of the selected provider ('name').
        /// </summary>
        public SampleSet(SetName name)
        {
            Parameters = new SampleSetParameters(name);
            SetSamples();
        }
        /// <summary>
        /// Get a SampleSet with customized SampleSetParameters.
        /// </summary>
        public SampleSet(SampleSetParameters set)
        {
            Parameters = set;
            SetSamples();
        }

        #region helpers

        void SetSamples()
        {
            // If not all paths exist locally

            if (Parameters.Paths.Values.Any(x => !File.Exists(x)))
            {
                // Log("Not all files could be found locally.")

                // try to download them

                try
                {
                    Enum.GetValues(typeof(SampleType))
                        .ForEach<SampleType>(x => Parameters.Paths[x] = GetFileFromUrl(x));
                }

                // or create them on the fly and return.

                catch (Exception)
                {
                    // Log($"{e.Message} (Maybe not all files could be found online.)")

                    TrainingSamples = CreateSamples(Parameters.TrainingSamples, Parameters.InputDistortion, Parameters.TargetTolerance);
                    TestingSamples = CreateSamples(Parameters.TestingSamples, Parameters.InputDistortion, Parameters.TargetTolerance);
                    return;
                }
            }

            // Get samples from local path.

            FileStream fs_trainLabels = new FileStream(Parameters.Paths[SampleType.TestingData], FileMode.Open);
            FileStream fs_trainData = new FileStream(Parameters.Paths[SampleType.TestingData], FileMode.Open);
            FileStream fs_testLabels = new FileStream(Parameters.Paths[SampleType.TestingData], FileMode.Open);
            FileStream fs_testData = new FileStream(Parameters.Paths[SampleType.TestingData], FileMode.Open);

            TrainingSamples = ConvertToSamples(fs_trainLabels, fs_trainData);
            TestingSamples = ConvertToSamples(fs_testLabels, fs_testData);
        }
        string GetFileFromUrl(SampleType sampleType)
        {
            string uri = Parameters.Paths[sampleType];
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
    }
}
