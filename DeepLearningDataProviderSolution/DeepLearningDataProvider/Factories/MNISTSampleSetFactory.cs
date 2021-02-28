using MatrixHelper;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DeepLearningDataProvider.Factories
{
    public class MNISTImage
    {
        public byte Label { get; set; }
        public byte[] Data { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }

    public class MNISTSampleSetFactory : SampleSetFactoryBase
    {
        #region CreateSamples

        protected override Sample[] CreateSamples(int samples, float inputDistortion, float targetTolerance)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ConvertFilesToSamples

        protected override Sample[] ConvertFilesToSamples(FileStream fs_labels, FileStream fs_imgs)
        {
            {
                MNISTImage[] imgs = GetImages(fs_labels, fs_imgs);
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

        #region helpers 

        /// <summary>
        /// Convert Stream to (MNIST) Image format.
        /// </summary>
        MNISTImage[] GetImages(FileStream labelsStream, FileStream imagesStream)
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
            MNISTImage[] result = new MNISTImage[numberOfImages];

            for (int i = 0; i < numberOfImages; i++)
            {
                result[i] = new MNISTImage
                {
                    Data = images.ReadBytes(width * height),
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
        Matrix GetExpectedOutput(MNISTImage[] imgs, int i)
        {
            Matrix expectedOutput = new Matrix(10);
            expectedOutput[imgs[i].Label] = 1;
            return expectedOutput;
        }
        /// <summary>
        /// 'Convert' Image.Data to Matrix
        /// </summary>
        Matrix GetInput(MNISTImage[] imgs, int i)
        {
            float[] dataAsFloatArray = Array.ConvertAll(imgs[i].Data, x => (float)x);
            Matrix input = new Matrix(dataAsFloatArray);
            return input;
        }
        /// <summary>
        /// 'Convert' Image.Data to two-dimensional Matrix
        /// </summary>
        Matrix GetRawInput(MNISTImage[] imgs, int i)
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

        internal ISampleSet CreateDefaultSampleSetAsync()
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region CreateDefaultSampleSet

        internal override Task<ISampleSet> CreateDefaultSampleSetAsync(SetName setName)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
