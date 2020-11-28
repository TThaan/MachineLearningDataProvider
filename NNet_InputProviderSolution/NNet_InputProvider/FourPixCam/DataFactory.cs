using MatrixHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NNet_InputProvider.FourPixCam
{
    public class DataFactory
    {
        #region ctor & fields

        static Random rnd;
        static Dictionary<Label, Matrix> rawInputs;
        static Dictionary<Label, Matrix> distortedInputs;
        static Dictionary<Label, Matrix> validInputs;
        static Dictionary<Label, Matrix> validOutputs;
        static Sample[] validSamples;

        #endregion

        #region public

        public static bool SaveTrainingDataToJson(int samples, float sampleTolerance, float distortion, string file)
        {
            var trDat = GetTrainingDataAsJson(samples, sampleTolerance, distortion);
            try
            {
                // File.CreateText(file);
                File.WriteAllText(file, trDat);
                return true;
            }
            catch (Exception)
            {
                throw;
                // return false;
            }
        }
        public static string GetTrainingDataAsJson(int samples, float sampleTolerance, float distortion)
        {
            var trDat = GetTrainingData(samples, sampleTolerance, distortion);
            return JsonConvert.SerializeObject(trDat, Formatting.Indented);
        }
        public static Sample[] GetTrainingData(int samples, float sampleTolerance, float distortion)
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
        public static Sample[] GetTestingData(int multiplyer)
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

        static Sample[] GetValidTrainingData(int sampleSize, Sample[] _validSamples)
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
        static Sample[] GetValidSamples()
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
        static Dictionary<Label, Matrix> GetRawInputs()
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
        static Dictionary<Label, Matrix> GetDistortedInputs(float d)
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
        static float GetDistortedValue(float distortionDeviation)
        {
            return 1f - (float)rnd.NextDouble() * distortionDeviation;
        }
        static Dictionary<Label, Matrix> GetValidInputs(Dictionary<Label, Matrix> _rawInputs)
        {
            var test = _rawInputs.ToDictionary(x => x.Key, x => Operations.FlattenToOneColumn(x.Value));
            return test;
        }
        static Dictionary<Label, Matrix> GetValidOutputs()
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
