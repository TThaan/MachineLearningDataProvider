using DeepLearningDataProvider.JsonConverters;
using MatrixExtensions;
using Newtonsoft.Json;
using System;
using static DeepLearningDataProvider.Factories.FourPixCamSampleSetFactory;

namespace DeepLearningDataProvider
{
    [Serializable]
    public class Sample
    {
        //[JsonConstructor]
        //public Sample()
        //{

        //}

        #region fields

        // bool isOutputCorrect;
        // IMatrix actualOutput;
        // ILogger _logger;

        #endregion

        #region public

        public static float Tolerance { get; set; } = 0;
        public int Id { get; internal set; }
        public Label Label { get; set; }    // FourPixCam dedicated so far..
        //[JsonConverter(typeof(GenericJsonConverter<float[]>))]
        //[JsonProperty(ItemConverterType = typeof(GenericJsonConverter<float>))]
        public dynamic RawInput { get; set; }
        //[JsonConverter(typeof(GenericJsonConverter<float[]>))]
        public float[] Input { get; set; }
        //[JsonConverter(typeof(GenericJsonConverter<float[]>))]
        public float[] ExpectedOutput { get; set; }

        //[JsonIgnore]
        //public IMatrix ActualOutput 
        //{
        //    get => actualOutput;
        //    set
        //    {
        //        // Always when 'ActualOutput' is (re)set change 'isOutputCorrect' to null.
        //        // isOutputCorrect = null;
        //        actualOutput = value;
        //    }
        //}
        // [JsonIgnore]
        // public bool IsOutputCorrect => IsOutputApproximatelyCorrect();
        // https://stackoverflow.com/a/25769147
        // throw new ArgumentException("You cannot check the output when there is no actual output.");

        #endregion

        #region helpers

        public bool IsOutputApproximatelyCorrect(float[] actualOutput)
        {
            int a = actualOutput.Length;
            int b = actualOutput.Length;

            if (a == b)
            {
                for (int j = 0; j < b; j++)
                {
                    var a_j = actualOutput[j];
                    var t_j = ExpectedOutput[j];
                    var x = Math.Abs(t_j - a_j);
                    if (x > Tolerance)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        #endregion
    }
}
