using DeepLearningDataProvider.SampleSetHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        /// <summary>
        /// Load samples from file and prepare the sample set for training.
        /// </summary>
        void Initialize(decimal split);
        void Reset();
        bool IsInitialized { get; }

        //PathBuilder PathBuilder { get; }

        //Task<bool> LoadSampleSetAsync(string samplesFileName, float testSamplesFraction, int columnIndex_Label, params int[] ignoredColumnIndeces);
        //Task<bool> SaveSampleSetAsync(string fileName, bool overWriteExistingFile = false);

        event DataProviderChangedEventHandler DataProviderChanged;
    }

    public class SampleSet : ISampleSet
    {
        #region ISampleSet

        public Sample[] Samples { get; set; }
        public Sample[] TestSet { get; set; }
        public Sample[] TrainSet { get; set; }
        public List<Sample> ArrangedTrainSet { get; set; } = new List<Sample>();
        public Dictionary<string, float[]> Targets { get; set; } = new Dictionary<string, float[]>();

        #region Init

        /// <summary>
        /// Load samples from file and prepare the sample set for training.
        /// </summary>
        public void Initialize(decimal split)
        {
            if (Samples == null || Samples.Count() == 0)
                throw new ArgumentException("To initialize the sample set samples have to be loaded into it.");

            this.Split(split);

            // result.OnDataProviderChanged("Successfully loaded samples.");

            IsInitialized = true;
        }
        public void Reset()
        {
            this.UnloadSamples();
            IsInitialized = false;
        }
        public bool IsInitialized { get; private set; }

        #endregion

        #endregion

        internal IEnumerable<IGrouping<string, Sample>> GroupedSamples { get; set; }
        internal Dictionary<string, NullableIntArray> GroupedAndRandomizedIndeces { get; set; }
        internal Dictionary<string, NullableIntArray> MultipliedGroupedAndRandomizedIndeces { get; set; } // unused?
        internal Dictionary<string, int> AppendedSamplesPerLabel { get; set; } = new Dictionary<string, int>();

        #region DataProviderEventHandler

        public event DataProviderChangedEventHandler DataProviderChanged;   // SampleSetChanged?
        internal void OnDataProviderChanged(string info)
        {
            DataProviderChanged?.Invoke(this, new DataProviderChangedEventArgs(info));
        }

        #endregion
    }
}