using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DeepLearningDataProvider.SampleSetHelpers
{
    public static class SaveAndLoad
    {
        /// <summary>
        /// Currently only csv is loaded, more is coming.
        /// </summary>
        public static async Task LoadSamplesAsync(this ISampleSet sampleSet, string samplesFileName, int columnIndex_Label, params int[] ignoredColumnIndeces)//, int columnIndex_Target = -1
        {
            await Task.Run(() =>
            {
                if (!samplesFileName.EndsWith(".csv"))
                    throw new ArgumentException("The samples file must be a .csv file.");

                // in initializer method?:
                // notifiedSampleSet.OnDataProviderChanged("Loading samples from file, please wait...");

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
                        {
                            if (float.TryParse(columns[colNr], NumberStyles.Float, CultureInfo.InvariantCulture, out float feature))
                                features.Add(feature);
                            else throw new ArgumentException(
                                "The file could not be interpreted as a correct .csv file with one or more columns of float numbers and one with labels.");
                        }
                    }

                    newSample.Features = features.ToArray();
                }

                return sampleSet;
            });
        }
        public static void UnloadSamples(this ISampleSet sampleSet)
        {
            sampleSet.ArrangedTrainSet.Clear();
            sampleSet.Targets.Clear();
            sampleSet.TrainSet = null;
            sampleSet.TestSet = null;
            sampleSet.Samples = null;
        }
        /// <summary>
        /// Save samples to a file (only data, not the whole sample set).
        /// </summary>
        public static async Task SaveSamplesAsync(this ISampleSet sampleSet, string fileName, bool overWriteExistingFile = false)
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
    }
}
