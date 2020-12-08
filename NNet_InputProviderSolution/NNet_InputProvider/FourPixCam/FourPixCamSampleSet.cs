using MatrixHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NNet_InputProvider.FourPixCam
{
    // In ..:
    public enum Label
    {
        Undefined, AllBlack, AllWhite, LeftBlack, LeftWhite, SlashBlack, SlashWhite, TopBlack, TopWhite
    }

    public class FourPixCamSampleSet : SampleSet
    {
        #region ctor & fields

        Random rnd;
        Dictionary<Label, Matrix> rawInputs, validInputs, validOutputs;
        Sample[] validSamples, allSamples;

        public FourPixCamSampleSet(SetName setName) : base(setName) { }
        public FourPixCamSampleSet(SampleSetParameters set) : base(set) { }

        #endregion

        #region SampleSet

        protected override Sample[] CreateSamples(int samplesCount, float inputDistortion, float targetTolerance)
        {
            rnd = RandomProvider.GetThreadRandom();
            Sample.Tolerance = targetTolerance;

            rawInputs = GetRawInputs();
            validInputs = GetValidInputs(rawInputs);
            validOutputs = GetValidOutputs();
            validSamples = GetValidSamples();
            allSamples = GetMultipliedSamples(validSamples, samplesCount, inputDistortion);
            DistortSamples(allSamples, inputDistortion);
            return allSamples;
        }
        protected override Sample[] ConvertToSamples(FileStream fs_labels, FileStream fs_imgs)
        {
            throw new NotImplementedException();
        }

        #region helpers

        // Actually: To be defined as object in Sample!
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
        Dictionary<Label, Matrix> GetValidInputs(Dictionary<Label, Matrix> _rawInputs)
        {
            return _rawInputs.ToDictionary(x => x.Key, x => Operations.FlattenToOneColumn(x.Value));
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
        Sample[] GetMultipliedSamples(Sample[] _validSamples, int sampleSize, float inputDistortion)
        {
            List<Sample> result = new List<Sample>();
            int multiplicationFactor = (int)Math.Round((double)sampleSize / rawInputs.Values.Count, 0);

            for (int i = 0; i < multiplicationFactor; i++)
            {
                result.AddRange(_validSamples.Select(x => new Sample()
                {
                    Label = x.Label,
                    RawInput = new Matrix(x.RawInput),
                    Input = new Matrix(x.Input),
                    ExpectedOutput = new Matrix(x.ExpectedOutput),
                }));
            }

            return result.Shuffle().ToArray();
        }
        void DistortSamples(Sample[] samples, float inputDistortion)
        {
            samples.ForEach<Sample>(x => x.Input.ForEach(y => y = GetDistortedValue(y, inputDistortion)));
        }
        float GetDistortedValue(float value, float inputDistortion)
        {
            var test = (float)rnd.NextDouble();
            var result = value * (1f - test * inputDistortion);
            return result;
        }

        #endregion

        #endregion
    }
}
