namespace DeepLearningDataProvider
{
    // Corresponds to 'InputData' and 'OutputData' in a standard ML.Net code.
    public class Sample
    {
        public string Label { get; set; }
        public float[] Features { get; set; }
        // public float[] Target { get; set; } // redundant?!
    }
}