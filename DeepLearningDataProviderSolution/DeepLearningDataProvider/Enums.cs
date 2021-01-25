namespace DeepLearningDataProvider
{
    public enum StorageFormat
    {
        Undefined, ByteArray, Idx3ubyte, Json
    }
    public enum SetName //ProviderName?TemplateName?
    {
        FourPixelCamera, MNIST,
        Custom
    }
    public enum SampleType
    {
        TestingData, TestingLabel, TrainingData, TrainingLabel
    }
}