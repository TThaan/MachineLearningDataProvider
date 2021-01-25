using System;
using System.IO;

namespace DeepLearningDataProvider.Custom
{
    public class CustomSampleSet : SampleSet
    {
        #region ctor & fields

        public CustomSampleSet(SetName setName) : base(setName) { }
        public CustomSampleSet(SampleSetParameters set) : base(set) { }

        #endregion

        #region SampleSet

        protected override Sample[] ConvertToSamples(FileStream fs_labels, FileStream fs_imgs)
        {
            throw new NotImplementedException();
        }
        protected override Sample[] CreateSamples(int samplesCount, float inputDistortion, float targetTolerance)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
