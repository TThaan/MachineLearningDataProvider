using DeepLearningDataProvider.Builders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DeepLearningDataProvider
{
    public interface ISampleSet
    {
        Sample[] TestSet { get; set; }
        Sample[] TrainSet { get; set; }
        //PathBuilder PathBuilder { get; }

        Task<bool> LoadSampleSetAsync(string samplesFileName, float testSamplesFraction, int columnIndex_Label, params int[] ignoredColumnIndeces);
        Task<bool> SaveSampleSetAsync(string fileName, bool overWriteExistingFile = false);

        event DataProviderChangedEventHandler DataProviderChanged;
    }

    public class SampleSet : ISampleSet
    {
        #region fields & ctor

        private PathBuilder pathBuilder;

        public SampleSet()
        {
            pathBuilder = new PathBuilder(OnDataProviderChanged);   // via DC?
        }

        #endregion

        #region properties

        public Sample[] TestSet { get; set; }
        public Sample[] TrainSet { get; set; }
        //public PathBuilder PathBuilder
        //{
        //    get
        //    {
        //        if (pathBuilder == null)
        //            OnDataProviderChanged("Paths are null");
        //        return pathBuilder;
        //    }
        //}

        #endregion

        #region methods

        public async Task<bool> LoadSampleSetAsync(string samplesFileName, float testSamplesFraction, int columnIndex_Label, params int[] ignoredColumnIndeces)//, int columnIndex_Target = -1
        {
            return await Task.Run(async () =>
            {
                try
                {
                    OnDataProviderChanged("Loading samples from file, please wait...");

                    IEnumerable<Sample> samples = await InterpretFileAsync(samplesFileName, columnIndex_Label);

                    int trainSamplesCount = (int)Math.Round(samples.Count() * (1 - testSamplesFraction), 0);
                    TrainSet = samples.Take(trainSamplesCount).ToArray();
                    TestSet = samples.Skip(trainSamplesCount).ToArray();

                    OnDataProviderChanged("Successfully loaded samples.");
                    return true;
                }
                catch (Exception e) { OnDataProviderChanged(e.Message); return false; }
            });
        }
        public async Task<bool> SaveSampleSetAsync(string fileName, bool overWriteExistingFile = false)
        {
            if (TrainSet == null)
            { OnDataProviderChanged("There are no training samples loaded in SampleSet."); return false; }
            if (TestSet == null)
            { OnDataProviderChanged("There are no testing samples loaded in SampleSet."); return false; }

            try
            {
                OnDataProviderChanged("Saving sample set, please wait...");

                await ImpEx.Export.SaveAsCSVAsync(
                    TrainSet.Concat(TestSet),
                    fileName,
                    TrainSet.First().Features.Length + 1,
                    overWriteExistingFile);

                OnDataProviderChanged("Successfully saved sample set.");
                return true;
            }
            catch (Exception e) { OnDataProviderChanged(e.Message); return false; }
        }

        #region helpers

        /// <summary>
        /// Currently only csv is loaded. More is coming.
        /// </summary>
        private async Task<IEnumerable<Sample>> InterpretFileAsync(string samplesFileName, int columnIndex_Label)
        {
            return await Task.Run(() =>
            {
                IEnumerable<string> lines = File.ReadLines(samplesFileName);
                List<Sample> result = new List<Sample>(lines.Count());

                // Get amount of feature columns

                int featureColumnsCount = lines.First().Where(x => x == ',').Count();

                // Get Samples (i.e. convert line to Sample?)
                foreach (var line in lines)
                {
                    List<float> features = new List<float>(featureColumnsCount);

                    Sample newSample = new Sample { Label = null, Features = new float[featureColumnsCount] };
                    result.Add(newSample);

                    string[] columns = line.Split(',');
                    for (int i = 0; i < columns.Length; i++)
                    {
                        if (i == columnIndex_Label)
                            newSample.Label = columns[i];
                        else
                            features.Add(float.Parse(columns[i], CultureInfo.InvariantCulture));
                    }

                    newSample.Features = features.ToArray();
                }

                return result;
            });
        }

        #endregion

        #endregion

        #region DataProviderEventHandler

        public event DataProviderChangedEventHandler DataProviderChanged;
        void OnDataProviderChanged(string info)
        {
            DataProviderChanged?.Invoke(this, new DataProviderChangedEventArgs(info));
        }

        #endregion
    }
}