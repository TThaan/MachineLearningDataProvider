using MatrixHelper;
using System;
using static DeepLearningDataProvider.Factories.FourPixCamSampleSetFactory;

namespace DeepLearningDataProvider
{
    [Serializable]
    public class Sample
    {
        #region fields

        // bool isOutputCorrect;
        // IMatrix actualOutput;
        // ILogger _logger;

        #endregion

        #region public

        public static float Tolerance { get; set; } = 0;
        public int Id { get; internal set; }
        public Label Label { get; set; }    // FourPixCam dedicated so far..
        public IMatrix RawInput { get; set; }
        public IMatrix Input { get; set; }
        public IMatrix ExpectedOutput { get; set; }

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

        public bool IsOutputApproximatelyCorrect(IMatrix actualOutput)
        {
            if (actualOutput.m == ExpectedOutput.m && actualOutput.n == 1 && ExpectedOutput.n == 1)
            {
                for (int j = 0; j < actualOutput.m; j++)
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
