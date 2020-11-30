using System;
using System.Collections.Generic;
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
        /// Order:
        /// Url_TrainingLabels, string Url_TrainingImages, string Url_TestingLabels, string Url_TestingImages
        /// </summary>
        public BaseDataFactory(params string[] urls)
        {
            if (urls.Length != 4)
                throw new ArgumentException("There must be exactly 4 urls/paths passed to (Base)DataFactory!");

            Paths = new Dictionary<Tuple<SamplePurpose, SampleProperty>, string>
            {
                [new Tuple<SamplePurpose, SampleProperty>(item1: SamplePurpose.Training, item2: SampleProperty.Label)] = urls[0],
                [new Tuple<SamplePurpose, SampleProperty>(item1: SamplePurpose.Training, item2: SampleProperty.Data)] = urls[1],
                [new Tuple<SamplePurpose, SampleProperty>(item1: SamplePurpose.Testing, item2: SampleProperty.Label)] = urls[2],
                [new Tuple<SamplePurpose, SampleProperty>(item1: SamplePurpose.Testing, item2: SampleProperty.Data)] = urls[3]
            };
        }

        #endregion

        #region public

        public Dictionary<Tuple<SamplePurpose, SampleProperty>, string> Paths { get; protected set; }

        public string Url_TrainImages => Paths.Values.ElementAt(0);
        public string Url_TrainLabels => Paths.Values.ElementAt(1);
        public string Url_TestImages => Paths.Values.ElementAt(2);
        public string Url_TestLabels => Paths.Values.ElementAt(3);

        public Sample[] TrainingSamples { get; protected set; }
        public Sample[] TestingSamples { get; protected set; }

        public abstract Sample[] CreateSamples(int samples, float inputDistortion, float targetTolerance);
        public Sample[] GetSamples(SamplePurpose purpose)
        {
            // Try to get samples from local files
            if (!(
                File.Exists(Paths.Single(x => x.Key.Item1 == purpose && x.Key.Item2 == SampleProperty.Label).Value) && 
                File.Exists(Paths.Single(x => x.Key.Item1 == purpose && x.Key.Item2 == SampleProperty.Data).Value)
                ))
            {
                // If not all local files were found under given paths/urls, 
                // try to get samples from web under those paths/urls 
                // and store them as local files.
                try
                {
                    Paths[new Tuple<SamplePurpose, SampleProperty> (item1:purpose, item2:SampleProperty.Label)] 
                        = GetFileFromUrl(purpose, SampleProperty.Label);
                    Paths[new Tuple<SamplePurpose, SampleProperty>(item1: purpose, item2: SampleProperty.Data)] 
                        = GetFileFromUrl(purpose, SampleProperty.Data);
                }
                catch (Exception)
                {
                    throw new FileNotFoundException("At least one of your paths represents neither a path to a local file nor an active web link.");
                }
            }

            // Try again to get samples from local files
            FileStream fs_labels = new FileStream(
                Paths[new Tuple<SamplePurpose, SampleProperty>(item1: purpose, item2: SampleProperty.Label)],
                FileMode.Open);
            FileStream fs_imgs = new FileStream(
                Paths[new Tuple<SamplePurpose, SampleProperty>(item1: purpose, item2: SampleProperty.Data)],
                FileMode.Open);

            if (purpose == SamplePurpose.Training)
            {
                return TrainingSamples = GetSamplesFromStream(fs_labels, fs_imgs);
            }
            else
            {
                return TestingSamples = GetSamplesFromStream(fs_labels, fs_imgs);
            }
        }

        #region Abstract

        // protected abstract Sample[] GetSamplesFromWeb(SamplePurpose purpose, SampleProperty prop);
        // protected abstract Sample[] GetSamplesFromLocalPath(string path_labels, string path_images);

        protected abstract Sample[] GetSamplesFromStream(FileStream fs_labels, FileStream fs_imgs);

        #endregion

        #endregion

        #region helpers

        string GetCurrentUrl(SamplePurpose purpose, SampleProperty prop)
        {
            string uri;
            if (purpose == SamplePurpose.Training)
            {
                if (prop == SampleProperty.Label)
                {
                    uri = Url_TrainLabels;
                }
                else
                {
                    uri = Url_TrainImages;
                }
            }
            else
            {
                if (prop == SampleProperty.Label)
                {
                    uri = Url_TestLabels;
                }
                else
                {
                    uri = Url_TestImages;
                }
            }

            return uri;
        }
        string GetFileFromUrl(SamplePurpose purpose, SampleProperty prop)
        {
            string uri = GetCurrentUrl(purpose, prop);
            string localPath = Path.GetTempPath() + purpose.ToString() + prop.ToString();

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

        // ...
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

        #endregion
    }
}
