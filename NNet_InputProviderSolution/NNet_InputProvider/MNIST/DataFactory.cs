using MatrixHelper;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace NNet_InputProvider.MNIST
{
    public class DataFactory : BaseDataFactory
    {
        #region ctor & fields

        static string 
            url_TrainLabels = "http://yann.lecun.com/exdb/mnist/train-images-idx3-ubyte.gz",
            url_TrainImages = "http://yann.lecun.com/exdb/mnist/train-labels-idx1-ubyte.gz",
            url_TestLabels = "http://yann.lecun.com/exdb/mnist/t10k-images-idx3-ubyte.gz",
            url_TestImages = "http://yann.lecun.com/exdb/mnist/t10k-labels-idx1-ubyte.gz";

        /// <summary>
        /// Order:
        /// Url_TrainingLabels, string Url_TrainingImages, string Url_TestingLabels, string Url_TestingImages
        /// </summary>
        public DataFactory(params string[] urls) : base(urls) { }
        /// <summary>
        /// Defining urls with last known addresses.
        /// </summary>
        public DataFactory()
        {
            Url_TrainLabels = url_TrainLabels;
            Url_TrainImages = url_TrainImages;
            Url_TestLabels = url_TestLabels;
            Url_TestImages = url_TestImages;
        }

        #endregion

        #region public

        public Sample[] GetTrainingSamples(float sampleTolerance, float distortion)
        {
            return GetSamples(Url_TrainLabels, Url_TrainImages, sampleTolerance, distortion, "training");
        }
        public Sample[] GetTestingSamples(float sampleTolerance, float distortion)
        {
            return GetSamples(Url_TestLabels, Url_TestImages, sampleTolerance, distortion, "testing");
        }

        #region default links

        #endregion

        #endregion

        #region helpers 

        Sample[] GetSamples(string labelsUri, string imgsUri, float sampleTolerance, float distortion, string prefix)
        {
            string path_labels = GetFileFromUrl(labelsUri, prefix, "labels");
            FileStream fs_labels = new FileStream(path_labels, FileMode.Open);
            string path_imgs = GetFileFromUrl(imgsUri, prefix, "images");
            FileStream fs_imgs = new FileStream(path_imgs, FileMode.Open);

            Image[] imgs = GetImages(fs_labels, fs_imgs);

            Sample[] result = new Sample[imgs.Length];
            Sample.Tolerance = sampleTolerance;

            for (int i = 0; i < imgs.Length; i++)
            {
                result[i] = new Sample()
                {
                    ExpectedOutput = GetExpectedOutput(imgs, i),
                    Input = GetInput(imgs, i),
                    RawInput = GetRawInput(imgs, i),
                };
            }

            return result;
        }
        /// <summary>
        /// Get data from url including saving it to disk (tmp directory).
        /// </summary>
        string DownloadFileAndGetPath(string uri)
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
                        fileStream.Write(buffer, 0 , buffer.Length);
                        // or without saving to disk

                    }
                }
            }

            return path;
        }
        /// <summary>
        /// Get data from url without saving it to disk.
        /// </summary>
        string GetFileFromUrl(string uri, string prefix, string suffix)
        {
            string path = Path.GetTempPath() + prefix + suffix;
            byte[] labelsArray = new WebClient().DownloadData(uri);

            using (var memStream = new MemoryStream(labelsArray))
            {
                using (var zipStream = new GZipStream(memStream, CompressionMode.Decompress))
                {
                    byte[] buffer = new byte[0x10000];

                    using (var fileStream = File.Create(path))
                    {
                        while (zipStream.Read(buffer, 0, buffer.Length) > 0)
                        {
                            fileStream.Write(buffer, 0, buffer.Length);
                        }
                        return path;
                    }
                }
            }
        }
        /// <summary>
        /// Convert Stream to (MNIST) Image format.
        /// </summary>
        Image[] GetImages(FileStream labelsStream, FileStream imagesStream)
        {
            BinaryReader labels = new BinaryReader(labelsStream);

            // Get metadata of  labels
            int magicLabel = labels.ReadBigInt32();
            int numberOfLabels = labels.ReadBigInt32();

            BinaryReader images = new BinaryReader(imagesStream);

            // Get metadata of images
            int magicNumber = images.ReadBigInt32();
            int numberOfImages = images.ReadBigInt32();
            int width = images.ReadBigInt32();
            int height = images.ReadBigInt32();

            // Get content
            Image[] result = new Image[numberOfImages];

            for (int i = 0; i < numberOfImages; i++)
            {
                result[i] = new Image
                {
                    Data = images.ReadBytes(width*height),
                    Label = labels.ReadByte(),
                    Width = width,
                    Height = height
                };
            }

            return result;
        }

        /// <summary>
        /// 'Convert' Image.Label to Matrix
        /// </summary>
        Matrix GetExpectedOutput(Image[] imgs, int i)
        {
            Matrix expectedOutput = new Matrix(10);
            expectedOutput[imgs[i].Label] = 1;
            return expectedOutput;
        }
        /// <summary>
        /// 'Convert' Image.Data to Matrix
        /// </summary>
        Matrix GetInput(Image[] imgs, int i)
        {
            float[] dataAsFloatArray = Array.ConvertAll(imgs[i].Data, x => (float)x);
            Matrix input = new Matrix(dataAsFloatArray);
            return input;
        }
        /// <summary>
        /// 'Convert' Image.Data to two-dimensional Matrix
        /// </summary>
        Matrix GetRawInput(Image[] imgs, int i)
        {
            Matrix rawData = new Matrix(imgs[i].Height, imgs[i].Width);
            for (int j = 0; j < rawData.m; j++)
            {
                for (int k = 0; k < rawData.n; k++)
                {
                    rawData[j, k] = imgs[i].Data[j * rawData.n + k];
                }
            }

            return rawData;
        }

        #endregion
    }
}
