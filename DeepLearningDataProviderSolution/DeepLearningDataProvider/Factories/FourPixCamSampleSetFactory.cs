using MatrixExtensions;
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

            Dictionary<Label, float[,]> rawInputs = GetRawInputs();
            Dictionary<Label, float[]> validInputs = GetValidInputs(rawInputs);
            Dictionary<Label, float[]> validOutputs = GetValidOutputs();
            Sample[] validSamples = GetValidSamples(rawInputs, validInputs, validOutputs);
            Sample[] allSamples = GetMultipliedSamples(rawInputs, validSamples, trainingSamplesCount, inputDistortion);
            DistortSamples(allSamples, inputDistortion);

            return allSamples;
        }

        #region helpers

        // Actually: To be defined as object in Sample!
        Dictionary<Label, float[,]> GetRawInputs()
        {
            return new Dictionary<Label, float[,]>
            {
                [Label.AllBlack] = new float[,] {
                    { -1, -1 },
                    { -1, -1 } },

                [Label.AllWhite] = new float[,] {
                    { 1, 1 },
                    { 1, 1 } },

                [Label.TopBlack] = new float[,] {
                    { -1, -1 },
                    { 1, 1 } },

                [Label.TopWhite] = new float[,] {
                    { 1, 1 },
                    { -1, -1 } },

                [Label.LeftBlack] = new float[,] {
                    { -1, 1 },
                    { -1, 1 } },

                [Label.LeftWhite] = new float[,] {
                    { 1, -1 },
                    { 1, -1 } },

                [Label.SlashBlack] = new float[,] {
                    { 1, -1 },
                    { -1, 1 } },

                [Label.SlashWhite] = new float[,] {
                    { -1, 1 },
                    { 1, -1 } }
            };
        }
        /// <summary>
        /// Flattening two-dimensional raw input into one dimensional input.
        /// </summary>
        Dictionary<Label, float[]> GetValidInputs(Dictionary<Label, float[,]> _rawInputs)
        {
            return _rawInputs.ToDictionary(x => x.Key, x => x.Value.ToList<float>().ToArray());
        }
        Dictionary<Label, float[]> GetValidOutputs()
        {
            return new Dictionary<Label, float[]>
            {
                [Label.AllWhite] = new float[] { 1, 0, 0, 0 },

                [Label.AllBlack] = new float[] { 1, 0, 0, 0 },

                [Label.TopWhite] = new float[] { 0, 1, 0, 0 },

                [Label.TopBlack] = new float[] { 0, 1, 0, 0 },

                [Label.LeftWhite] = new float[] { 0, 0, 1, 0 },

                [Label.LeftBlack] = new float[] { 0, 0, 1, 0 },

                [Label.SlashWhite] = new float[] { 0, 0, 0, 1 },

                [Label.SlashBlack] = new float[] { 0, 0, 0, 1 }
            };
        }
        Sample[] GetValidSamples(Dictionary<Label, float[,]> rawInputs, Dictionary<Label, float[]> validInputs, Dictionary<Label, float[]> validOutputs)
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
        Sample[] GetMultipliedSamples(Dictionary<Label, float[,]> rawInputs, Sample[] _validSamples, int sampleSize, float inputDistortion)
        {
            List<Sample> result = new List<Sample>();
            int multiplicationFactor = (int)Math.Round((double)sampleSize / rawInputs.Values.Count, 0);

            for (int i = 0; i < multiplicationFactor; i++)
            {
                result.AddRange(_validSamples.Select((x, index) => new Sample()
                {
                    Id = _validSamples.Length * i + index,
                    Label = x.Label,
                    RawInput = ((float[,])x.RawInput).GetCopy(),
                    Input = x.Input.GetCopy(),
                    ExpectedOutput = x.ExpectedOutput.GetCopy(),
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
