using MatrixHelper;
using NNet_InputProvider.FourPixCam;
using System;
using System.Text.Json.Serialization;

namespace NNet_InputProvider
{
    [Serializable]
    public class Sample // : ISample
    {
        #region fields

        bool isOutputCorrect;
        IMatrix actualOutput;

        #endregion

        #region public

        public static float Tolerance { get; set; } = 0;
        public int Id { get; internal set; }
        public Label Label { get; set; }
        public IMatrix RawInput { get; set; }
        public IMatrix Input { get; set; }
        public IMatrix ExpectedOutput { get; set; }

        [JsonIgnore]
        public IMatrix ActualOutput 
        {
            get => actualOutput;
            set
            {
                // Always when 'ActualOutput' is (re)set change 'isOutputCorrect' to null.
                // isOutputCorrect = null;
                actualOutput = value;
            }
        }
        [JsonIgnore]
        public bool IsOutputCorrect => IsOutputApproximatelyCorrect();
        // https://stackoverflow.com/a/25769147
        // throw new ArgumentException("You cannot check the output when there is no actual output.");

        #endregion

        #region helpers

        bool IsOutputApproximatelyCorrect()
        {
            if (ActualOutput.m == ExpectedOutput.m && ActualOutput.n == 1 && ExpectedOutput.n == 1)
            {
                for (int j = 0; j < ActualOutput.m; j++)
                {
                    var a_j = ActualOutput[j];
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
