using MatrixHelper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NNet_InputProvider.FourPixCam
{
    public enum Label
    {
        Undefined, AllBlack, AllWhite, LeftBlack, LeftWhite, SlashBlack, SlashWhite, TopBlack, TopWhite
    }

    public class DataFactory : BaseDataFactory
    {
        #region ctor & fields

        static string
            url_TrainLabels = "",
            url_TrainImages = "",
            url_TestLabels = "",
            url_TestImages = "";

        Random rnd;
        Dictionary<Label, Matrix> rawInputs;
        Dictionary<Label, Matrix> distortedInputs;
        Dictionary<Label, Matrix> validInputs;
        Dictionary<Label, Matrix> validOutputs;
        Sample[] validSamples;

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

        public Sample[] GetTrainingSamples(int samples, float sampleTolerance, float distortion)
        {
            rnd = RandomProvider.GetThreadRandom();

            rawInputs = GetRawInputs();
            distortedInputs = GetDistortedInputs(distortion);
            validInputs = GetValidInputs(distortedInputs);
            validOutputs = GetValidOutputs();
            Sample.Tolerance = sampleTolerance;
            validSamples = GetValidSamples();

            return GetValidTrainingData(samples, validSamples);
        }
        public Sample[] GetTestingSamples(int multiplyer)
        {
            var result = new List<Sample>();
            for (int i = 0; i < multiplyer; i++)
            {
                result.AddRange(validSamples);
            }
            return result.ToArray();
        }

        #endregion

        #region helper methods

        Sample[] GetValidTrainingData(int sampleSize, Sample[] _validSamples)
        {
            List<Sample> tmpResult = new List<Sample>();
            int amountOfCompleteSampleSets = (int)Math.Round((double)sampleSize / rawInputs.Values.Count, 0);

            for (int i = 0; i < amountOfCompleteSampleSets; i++)
            {
                tmpResult.AddRange(_validSamples);
            }
            Sample[] result = tmpResult.Shuffle().ToArray();

            return result;
        }
        Sample[] GetValidSamples()
        {
            var result = new List<Sample>();

            var labels = Enum.GetValues(typeof(Label)).ToList<Label>().Skip(1);
            foreach (var label in labels)
            {
                result.Add(new Sample
                {
                    Label = label,
                    RawInput = rawInputs[label],
                    Input = validInputs[label],
                    ExpectedOutput = validOutputs[label]
                });
            }

            return result.ToArray();
        }
        Dictionary<Label, Matrix> GetRawInputs()
        {
            return new Dictionary<Label, Matrix>
            {
                [Label.AllBlack] = new Matrix(new float[,] {
                    { -1, -1 },
                    { -1, -1 } }),

                [Label.AllWhite] = new Matrix(new float[,] {
                    { 1, 1 },
                    { 1, 1 } }),

                [Label.TopBlack] = new Matrix(new float[,] {
                    { -1, -1 },
                    { 1, 1 } }),

                [Label.TopWhite] = new Matrix(new float[,] {
                    { 1, 1 },
                    { -1, -1 } }),

                [Label.LeftBlack] = new Matrix(new float[,] {
                    { -1, 1 },
                    { -1, 1 } }),

                [Label.LeftWhite] = new Matrix(new float[,] {
                    { 1, -1 },
                    { 1, -1 } }),

                [Label.SlashBlack] = new Matrix(new float[,] {
                    { 1, -1 },
                    { -1, 1 } }),

                [Label.SlashWhite] = new Matrix(new float[,] {
                    { -1, 1 },
                    { 1, -1 } })
            };
        }
        Dictionary<Label, Matrix> GetDistortedInputs(float d)
        {
            return new Dictionary<Label, Matrix>
            {
                [Label.AllBlack] = new Matrix(new float[,] {
                    { -(GetDistortedValue(d)), -(GetDistortedValue(d)) },
                    { -(GetDistortedValue(d)), -(GetDistortedValue(d)) } }),

                [Label.AllWhite] = new Matrix(new float[,] {
                    { (GetDistortedValue(d)), (GetDistortedValue(d)) },
                    { (GetDistortedValue(d)), (GetDistortedValue(d)) } }),

                [Label.TopBlack] = new Matrix(new float[,] {
                    { -(GetDistortedValue(d)), -(GetDistortedValue(d)) },
                    { (GetDistortedValue(d)), (GetDistortedValue(d)) } }),

                [Label.TopWhite] = new Matrix(new float[,] {
                    { (GetDistortedValue(d)), (GetDistortedValue(d)) },
                    { -(GetDistortedValue(d)), -(GetDistortedValue(d)) } }),

                [Label.LeftBlack] = new Matrix(new float[,] {
                    { -(GetDistortedValue(d)), (GetDistortedValue(d)) },
                    { -(GetDistortedValue(d)), (GetDistortedValue(d)) } }),

                [Label.LeftWhite] = new Matrix(new float[,] {
                    { (GetDistortedValue(d)), -(GetDistortedValue(d)) },
                    { (GetDistortedValue(d)), -(GetDistortedValue(d)) } }),

                [Label.SlashBlack] = new Matrix(new float[,] {
                    { (GetDistortedValue(d)), -(GetDistortedValue(d)) },
                    { -(GetDistortedValue(d)), (GetDistortedValue(d)) } }),

                [Label.SlashWhite] = new Matrix(new float[,] {
                    { -(GetDistortedValue(d)), (GetDistortedValue(d)) },
                    { (GetDistortedValue(d)), -(GetDistortedValue(d)) } })
            };
        }
        float GetDistortedValue(float distortionDeviation)
        {
            return 1f - (float)rnd.NextDouble() * distortionDeviation;
        }
        Dictionary<Label, Matrix> GetValidInputs(Dictionary<Label, Matrix> _rawInputs)
        {
            var test = _rawInputs.ToDictionary(x => x.Key, x => Operations.FlattenToOneColumn(x.Value));
            return test;
        }
        Dictionary<Label, Matrix> GetValidOutputs()
        {
            return new Dictionary<Label, Matrix>
            {
                [Label.AllWhite] = new Matrix(new float[] { 1, 0, 0, 0 }),

                [Label.AllBlack] = new Matrix(new float[] { 1, 0, 0, 0 }),

                [Label.TopWhite] = new Matrix(new float[] { 0, 1, 0, 0 }),

                [Label.TopBlack] = new Matrix(new float[] { 0, 1, 0, 0 }),

                [Label.LeftWhite] = new Matrix(new float[] { 0, 0, 1, 0 }),

                [Label.LeftBlack] = new Matrix(new float[] { 0, 0, 1, 0 }),

                [Label.SlashWhite] = new Matrix(new float[] { 0, 0, 0, 1 }),

                [Label.SlashBlack] = new Matrix(new float[] { 0, 0, 0, 1 })
            };
        }

        #endregion
    }
}
