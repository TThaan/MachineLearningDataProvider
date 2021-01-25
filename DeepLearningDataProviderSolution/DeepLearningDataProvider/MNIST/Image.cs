namespace DeepLearningDataProvider.MNIST
{
    public class Image
    {
        public byte Label { get; set; }
        public byte[] Data { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }
}
