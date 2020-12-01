namespace NNet_InputProvider
{
    public enum StorageFormat
    {
        Undefined, ByteArray, Idx3ubyte, Json
    }
    public enum SetName //ProviderName?TemplateName?
    {
        FourPixelCamera, MNIST
    }
    public enum SampleType
    {
        TestingData, TestingLabel, TrainingData, TrainingLabel
    }
}