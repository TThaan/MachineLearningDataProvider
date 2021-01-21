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
        Dictionary<Label, IMatrix> rawInputs, validInputs, validOutputs;
        Sample[] validSamples, allSamples;

        public FourPixCamSampleSet(SetName setName) : base(setName) { }
        public FourPixCamSampleSet(SampleSetParameters set) : base(set) { }

        #endregion

        #region SampleSet

        protected override Sample[] CreateSamples(int trainingSamplesCount, float inputDistortion, float targetTolerance)
        {
            rnd = RandomProvider.GetThreadRandom();
            Sample.Tolerance = targetTolerance;

            rawInputs = GetRawInputs();
            validInputs = GetValidInputs(rawInputs);
            validOutputs = GetValidOutputs();
            validSamples = GetValidSamples();
            allSamples = GetMultipliedSamples(validSamples, trainingSamplesCount, inputDistortion);
            DistortSamples(allSamples, inputDistortion);

            // debug
            //allSamples[0].RawInput = new Matrix(new float[,] {
            //        { -1, -1 },
            //        { -1, -1 } });
            //allSamples[0].Input[0] = -.85f;
            //allSamples[0].Input[1] = -.95f;
            //allSamples[0].Input[2] = -.89f;
            //allSamples[0].Input[3] = -.90f;
            //allSamples[0].ExpectedOutput[0] = 1;
            //allSamples[0].ExpectedOutput[1] = 0;
            //allSamples[0].ExpectedOutput[2] = 0;
            //allSamples[0].ExpectedOutput[3] = 0;
            //allSamples[0].Label = Label.AllBlack;

            return allSamples;
        }
        protected override Sample[] ConvertToSamples(FileStream fs_labels, FileStream fs_imgs)
        {
            throw new NotImplementedException();
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
            List<Sample> resultList = new List<Sample>();
            int multiplicationFactor = (int)Math.Round((double)sampleSize / rawInputs.Values.Count, 0);

            for (int i = 0; i < multiplicationFactor; i++)
            {
                resultList.AddRange(_validSamples.Select((x, index) => new Sample()
                {
                    Id = _validSamples.Length * i + index,
                    Label = x.Label,
                    RawInput = new Matrix(x.RawInput, "Sample.RawInput"),
                    Input = new Matrix(x.Input, "Sample.Input"),
                    ExpectedOutput = new Matrix(x.ExpectedOutput, "Sample.Output"),
                }));
            }

            // debug
            var idTest_List = resultList.GroupBy(x => x.Id);
            bool hm = idTest_List.All(y => y.Count() == 1);

            var resultArray = resultList.Shuffle().ToArray();
            var idTest_Array = resultArray.GroupBy(x => x.Id);
            bool hm2 = idTest_Array.All(y => y.Count() == 1);
            return resultArray;
        }
        void DistortSamples(Sample[] samples, float inputDistortion)
        {
            samples.ForEach<Sample>(x => x.Input.ForEach(y => y = GetDistortedValue(y, inputDistortion)));
        }
        float GetDistortedValue(float value, float inputDistortion)
        {
            double test = value * (1f - rnd.NextDouble() * inputDistortion);
            // float result = (float)Math.Round(test, 2);
            return (float)test;
        }

        #endregion

        #endregion
    }
}
