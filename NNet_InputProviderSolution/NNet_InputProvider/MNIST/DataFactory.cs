using MatrixHelper;
using NNet_InputProvider.FourPixCam;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace NNet_InputProvider.MNIST
{
    // ta: Sample.Tolerance

    public class DataFactory
    {
        public static Sample[] GetSamples(string labelsUri, string imgsUri, float sampleTolerance = 0f)
        {
            FileStream fs_labels = GetStreamFromUrl(labelsUri);
            FileStream fs_imgs = GetStreamFromUrl(imgsUri);

            Image[] imgs = GetImages(fs_labels, fs_imgs);

            Sample[] result = new Sample[imgs.Length]; ;

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

        #region helpers 

        /// <summary>
        /// Get data from url including saving it to disk (tmp directory).
        /// </summary>
        static string DownloadFileAndGetPath(string uri)
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
        static FileStream GetStreamFromUrl(string uri)
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
                        return fileStream;
                    }
                }
            }
        }
        /// <summary>
        /// Convert Stream to (MNIST) Image format.
        /// </summary>
        static Image[] GetImages(FileStream labelsStream, FileStream imagesStream)
        {
            BinaryReader images = new BinaryReader(imagesStream);

            // Get metadata of images
            int magicNumber = images.ReadBigInt32();
            int numberOfImages = images.ReadBigInt32();
            int width = images.ReadBigInt32();
            int height = images.ReadBigInt32();

            BinaryReader labels = new BinaryReader(labelsStream);

            // Get metadata of  labels
            int magicLabel = labels.ReadBigInt32();
            int numberOfLabels = labels.ReadBigInt32();

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
        static Matrix GetExpectedOutput(Image[] imgs, int i)
        {
            Matrix expectedOutput = new Matrix(10);
            expectedOutput[imgs[i].Label] = 1;
            return expectedOutput;
        }
        /// <summary>
        /// 'Convert' Image.Data to Matrix
        /// </summary>
        static Matrix GetInput(Image[] imgs, int i)
        {
            float[] dataAsFloatArray = Array.ConvertAll(imgs[i].Data, x => (float)x);
            Matrix input = new Matrix(dataAsFloatArray);
            return input;
        }
        /// <summary>
        /// 'Convert' Image.Data to two-dimensional Matrix
        /// </summary>
        static Matrix GetRawInput(Image[] imgs, int i)
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
