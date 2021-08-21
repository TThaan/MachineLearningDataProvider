using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DeepLearningDataProvider.SampleSetHelpers
{
    public static class SaveAndLoad
    {
        /// <summary>
        /// Currently only csv is loaded, more is coming.
        /// </summary>
        public static async Task<SampleSet> LoadSampleSetAsync(string samplesFileName, decimal split, int columnIndex_Label, params int[] ignoredColumnIndeces)//, int columnIndex_Target = -1
        {
            return await Task.Run(() =>
            {
                if (!samplesFileName.EndsWith(".csv"))
                    throw new ArgumentException("The samples file must be a .csv file.");

                SampleSet result = new SampleSet();

                // in initializer method?:
                // notifiedSampleSet.OnDataProviderChanged("Loading samples from file, please wait...");

                string[] lines = File.ReadLines(samplesFileName).ToArray();
                result.Samples = new Sample[lines.Count()];

                // Get amount of feature columns

                int featureColumnsCount = lines.First().Where(x => x == ',').Count();

                // Get Samples (i.e. convert line to Sample?)
                for (int lineNr = 0; lineNr < lines.Length; lineNr++)
                {
                    List<float> features = new List<float>(featureColumnsCount);

                    Sample newSample = new Sample { Label = null, Features = new float[featureColumnsCount] };
                    result.Samples[lineNr] = newSample;

                    string[] columns = lines.ElementAt(lineNr).Split(',');
                    for (int colNr = 0; colNr < columns.Length; colNr++)
                    {
                        if (colNr == columnIndex_Label)
                        {
                            newSample.Label = columns[colNr];
                            if (!result.Targets.Keys.Contains(newSample.Label))
                                result.Targets[newSample.Label] = null;
                        }
                        else
                        {
                            if (float.TryParse(columns[colNr], NumberStyles.Float, CultureInfo.InvariantCulture, out float feature))
                                features.Add(feature);
                            else throw new ArgumentException(
                                "The file could not be interpreted as a correct .csv file with one or more columns of float numbers and one with labels.");
                        }
                    }

                    newSample.Features = features.ToArray();
                }

                MapLabelsToTargets(result.Targets);
                result.Split(split);

                // throw new System.ArgumentException($"SaveAndLoad: {sampleSet.Samples.Length} {sampleSet.TestSet.Length}");
                // result.OnDataProviderChanged("Successfully loaded samples.");

                return result;
            });
        }
        public static async Task UnloadSampleSetAsync(ISampleSet sampleSet)
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
        public static async Task SaveSampleSetAsync(ISampleSet sampleSet, string fileName, bool overWriteExistingFile = false)
        {
            if (sampleSet.TrainSet == null)
            { throw new ArgumentException("There are no training samples loaded in SampleSet."); }
            if (sampleSet.TestSet == null)
            { throw new ArgumentException("There are no testing samples loaded in SampleSet."); }

            // in initializer method?:
            // notifiedSampleSet.OnDataProviderChanged("Saving sample set, please wait...");

            await ImpEx.Export.SaveAsCSVAsync(
                sampleSet.TrainSet.Concat(sampleSet.TestSet),
                fileName,
                sampleSet.TrainSet.First().Features.Length + 1,
                overWriteExistingFile);

            // notifiedSampleSet.OnDataProviderChanged("Successfully saved sample set.");
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
