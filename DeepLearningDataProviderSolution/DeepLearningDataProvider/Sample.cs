using Microsoft.ML.Data;

namespace DeepLearningDataProvider
{
    // How to have dynamic size of a vector property:
    // https://github.com/dotnet/machinelearning/issues/164#issuecomment-401200501
    // or: https://stackoverflow.com/a/24413055/10547243

    // [Serializable]
    // Corresponds to 'InputData' and 'OutputData' in a standard MLNet code.
    public class Sample
    {
        //private static int _x;

        //public Sample(int x)
        //{
        //    _x = x;
        //}

        //[LoadColumn(_x)]
        //[LoadColumn(0)]
        public string Label { get; set; }
        public float[] Features { get; set; }
        // public float[] Target { get; set; } // redundant?!
    }
}
