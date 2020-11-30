using MatrixHelper;
using System;
using System.IO;

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
        public DataFactory() : base(url_TrainLabels, url_TrainImages, url_TestLabels, url_TestImages) { }

        #endregion

        #region BaseDataFactory

        public override Sample[] CreateSamples(int samples, float inputDistortion, float targetTolerance)
        {
            throw new NotImplementedException();
        }
        protected override Sample[] GetSamplesFromStream(FileStream fs_labels, FileStream fs_imgs)
        {
            {
                Image[] imgs = GetImages(fs_labels, fs_imgs);
                Sample[] result = new Sample[imgs.Length];

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
        }

        #endregion

        #region (child class dedicated) helpers 

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
