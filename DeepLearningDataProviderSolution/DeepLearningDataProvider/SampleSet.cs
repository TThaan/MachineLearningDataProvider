using System.Collections.Generic;
using System.Linq;

namespace DeepLearningDataProvider
{
    public interface ISampleSet
    {
        //int Count { get; set; }
        // decimal TestFraction { get; set; }
        Sample[] Samples { get; set; }
        Sample[] TestSet { get; set; }
        Sample[] TrainSet { get; set; }
        List<Sample> ArrangedTrainSet { get; set; }
        Dictionary<string, float[]> Targets { get; set; }
        //PathBuilder PathBuilder { get; }

        //Task<bool> LoadSampleSetAsync(string samplesFileName, float testSamplesFraction, int columnIndex_Label, params int[] ignoredColumnIndeces);
        //Task<bool> SaveSampleSetAsync(string fileName, bool overWriteExistingFile = false);

        event DataProviderChangedEventHandler DataProviderChanged;
    }

    public class SampleSet : ISampleSet
    {
        #region fields & ctor

        // private PathBuilder pathBuilder;

        public SampleSet()
        {
            //pathBuilder = new PathBuilder(OnDataProviderChanged);   // via DC?
        }

        #endregion

        #region properties

        #region ISampleSet

        //public int Count { get; set; }
        // public decimal TestFraction { get; set; }
        public Sample[] Samples { get; set; }
        public Sample[] TestSet { get; set; }
        public Sample[] TrainSet { get; set; }
        public List<Sample> ArrangedTrainSet { get; set; } = new List<Sample>();
        public Dictionary<string, float[]> Targets { get; set; } = new Dictionary<string, float[]>();

        #endregion

        internal IEnumerable<IGrouping<string, Sample>> GroupedSamples { get; set; }
        internal Dictionary<string, NullableIntArray> GroupedAndRandomizedIndeces { get; set; }
        internal Dictionary<string, NullableIntArray> MultipliedGroupedAndRandomizedIndeces { get; set; } // unused?
        internal Dictionary<string, int> AppendedSamplesPerLabel { get; set; } = new Dictionary<string, int>();

        #endregion

        #region methods

        #endregion

        #region DataProviderEventHandler

        public event DataProviderChangedEventHandler DataProviderChanged;   // SampleSetChanged?
        internal void OnDataProviderChanged(string info)
        {
            DataProviderChanged?.Invoke(this, new DataProviderChangedEventArgs(info));
        }

        #endregion
    }
}