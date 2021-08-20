using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DeepLearningDataProvider.SampleSetExtensionMethods
{
    public static class SaveAndLoad
    {
        /// <summary>
        /// Currently only csv is loaded, more is coming.
        /// </summary>
        public static async Task LoadAsync(this ISampleSet sampleSet, string samplesFileName, decimal split, int columnIndex_Label, params int[] ignoredColumnIndeces)//, int columnIndex_Target = -1
        {
            await Task.Run(() =>
            {
                // Just because to access SampleSet's event..
                SampleSet notifiedSampleSet = sampleSet as SampleSet;

                // in try:
                notifiedSampleSet.OnDataProviderChanged("Loading samples from file, please wait...");

                // var result = new SampleSet();
                string[] lines = File.ReadLines(samplesFileName).ToArray();
                sampleSet.Samples = new Sample[lines.Count()];

                // Get amount of feature columns

                int featureColumnsCount = lines.First().Where(x => x == ',').Count();

                // Get Samples (i.e. convert line to Sample?)
                for (int lineNr = 0; lineNr < lines.Length; lineNr++)
                {
                    List<float> features = new List<float>(featureColumnsCount);

                    Sample newSample = new Sample { Label = null, Features = new float[featureColumnsCount] };
                    sampleSet.Samples[lineNr] = newSample;

                    string[] columns = lines.ElementAt(lineNr).Split(',');
                    for (int colNr = 0; colNr < columns.Length; colNr++)
                    {
                        if (colNr == columnIndex_Label)
                        {
                            newSample.Label = columns[colNr];
                            if (!sampleSet.Targets.Keys.Contains(newSample.Label))
                                sampleSet.Targets[newSample.Label] = null;
                        }
                        else
                            features.Add(float.Parse(columns[colNr], CultureInfo.InvariantCulture));
                    }

                    newSample.Features = features.ToArray();
                }

                MapLabelsToTargets(sampleSet.Targets);
                sampleSet.Split(split);

                // throw new System.ArgumentException($"SaveAndLoad: {sampleSet.Samples.Length} {sampleSet.TestSet.Length}");
                notifiedSampleSet.OnDataProviderChanged("Successfully loaded samples.");
            });
        }
        public static async Task UnloadAsync(this ISampleSet sampleSet)
        {
            await Task.Run(() => 
            {
                sampleSet.ArrangedTrainSet.Clear();
                sampleSet.Targets.Clear();
                sampleSet.TrainSet = null;
                sampleSet.TestSet = null;
                sampleSet.Samples = null;
            });
        }
        public static async Task SaveAsync(this ISampleSet sampleSet, string fileName, bool overWriteExistingFile = false)
        {
            // Just because to access SampleSet's event..
            SampleSet notifiedSampleSet = sampleSet as SampleSet;

            if (sampleSet.TrainSet == null)
            { throw new ArgumentException("There are no training samples loaded in SampleSet."); }
            if (sampleSet.TestSet == null)
            { throw new ArgumentException("There are no testing samples loaded in SampleSet."); }

            // in try:
            notifiedSampleSet.OnDataProviderChanged("Saving sample set, please wait...");

            await ImpEx.Export.SaveAsCSVAsync(
                sampleSet.TrainSet.Concat(sampleSet.TestSet),
                fileName,
                sampleSet.TrainSet.First().Features.Length + 1,
                overWriteExistingFile);

            notifiedSampleSet.OnDataProviderChanged("Successfully saved sample set.");
        }

        #region helpers

        private static void MapLabelsToTargets(Dictionary<string, float[]> targets)
        {
            int labelsCount = targets.Count;

            for (int i = 0; i < labelsCount; i++)
            {
                var key = targets.Keys.ElementAt(i);
                targets[key] = new float[labelsCount];
                targets[key][i] = 1;
            }
        }

        #endregion
    }
}
