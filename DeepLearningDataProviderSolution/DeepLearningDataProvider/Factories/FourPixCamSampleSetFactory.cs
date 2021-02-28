using MatrixHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DeepLearningDataProvider.Factories
{
    public class FourPixCamSampleSetFactory : SampleSetFactoryBase
    {
        public enum Label
        {
            Undefined, AllBlack, AllWhite, LeftBlack, LeftWhite, SlashBlack, SlashWhite, TopBlack, TopWhite
        }

        #region CreateSamples

        protected override Sample[] CreateSamples(int trainingSamplesCount, float inputDistortion, float targetTolerance)
        {
            rnd = RandomProvider.GetThreadRandom();
            Sample.Tolerance = targetTolerance;

            Dictionary<Label, IMatrix> rawInputs = GetRawInputs();
            Dictionary<Label, IMatrix> validInputs = GetValidInputs(rawInputs);
            Dictionary<Label, IMatrix> validOutputs = GetValidOutputs();
            Sample[] validSamples = GetValidSamples(rawInputs, validInputs, validOutputs);
            Sample[] allSamples = GetMultipliedSamples(rawInputs, validSamples, trainingSamplesCount, inputDistortion);
            DistortSamples(allSamples, inputDistortion);

            return allSamples;
        }

        #region helpers

        // Actually: To be defined as object in Sample!
        Dictionary<Label, IMatrix> GetRawInputs()
        {
            return new Dictionary<Label, IMatrix>
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
        Dictionary<Label, IMatrix> GetValidInputs(Dictionary<Label, IMatrix> _rawInputs)
        {
            return _rawInputs.ToDictionary(x => x.Key, x => Operations.FlattenToOneColumn(x.Value));
        }
        Dictionary<Label, IMatrix> GetValidOutputs()
        {
            return new Dictionary<Label, IMatrix>
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
        Sample[] GetValidSamples(Dictionary<Label, IMatrix> rawInputs, Dictionary<Label, IMatrix> validInputs, Dictionary<Label, IMatrix> validOutputs)
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
        Sample[] GetMultipliedSamples(Dictionary<Label, IMatrix> rawInputs, Sample[] _validSamples, int sampleSize, float inputDistortion)
        {
            List<Sample> result = new List<Sample>();
            int multiplicationFactor = (int)Math.Round((double)sampleSize / rawInputs.Values.Count, 0);

            for (int i = 0; i < multiplicationFactor; i++)
            {
                result.AddRange(_validSamples.Select((x, index) => new Sample()
                {
                    Id = _validSamples.Length * i + index,
                    Label = x.Label,
                    RawInput = new Matrix(x.RawInput, "Sample.RawInput"),
                    Input = new Matrix(x.Input, "Sample.Input"),
                    ExpectedOutput = new Matrix(x.ExpectedOutput, "Sample.Output"),
                }));
            }

            return result.ToArray();
        }
        void DistortSamples(Sample[] samples, float inputDistortion)
        {
            samples.ForEach<Sample>(x => x.Input.ForEach(y => y = GetDistortedValue(y, inputDistortion)));
        }
        float GetDistortedValue(float value, float inputDistortion)
        {
            return (float)(value * (1f - rnd.NextDouble() * inputDistortion));
        }

        #endregion

        #endregion

        #region ConvertFilesToSamples

        protected override Sample[] ConvertFilesToSamples(FileStream fs_labels, FileStream fs_imgs)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region CreateDefaultSampleSet

        internal override Task<ISampleSet> CreateDefaultSampleSetAsync(SetName setName)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
