using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeepLearningDataProvider.SampleSetExtensionMethods
{
    public static class SampleArranging
    {
        public static void ShuffleAllSamples(this ISampleSet sampleSet)
        {
            sampleSet.Samples.Shuffle(); 
        }
        public static void ShuffleTrainSet(this ISampleSet sampleSet)
        {
            sampleSet.TrainSet.Shuffle();
        }
        public static void Split(this ISampleSet sampleSet, decimal split) 
        {
            int trainSamplesCount = (int)Math.Round(sampleSet.Samples.Count() * (1 - split), 0);
            sampleSet.TrainSet = sampleSet.Samples.Take(trainSamplesCount).ToArray();
            sampleSet.TestSet = sampleSet.Samples.Skip(trainSamplesCount).ToArray();
        }
        public static async Task ArrangeSamplesAsync(this ISampleSet sampleSet, bool shuffleSamples, Dictionary<string, int> testResult, bool equalizeGroupSizes = true)
        {
            await Task.Run(() =>
            {
                decimal injSetFraction = .5m;   // make dynamic
                SampleSet set = sampleSet as SampleSet;

                SetAppendedSamplesPerLabel(set, injSetFraction, testResult);

                if (set.GroupedSamples == null)
                    set.GroupedSamples = set.TrainSet.GroupBy(x => x.Label);

                set.GroupedAndRandomizedIndeces = set.GroupedSamples
                    .ToDictionary(group => group.Key, group => new NullableIntArray(
                        GetRandomIndeces(set, group, equalizeGroupSizes),
                        set.AppendedSamplesPerLabel.Keys.Contains(group.Key) ? set.AppendedSamplesPerLabel[group.Key] : 0));
                SetArrangedTrainSet(set);

                set.SamplesTotal = set.ArrangedTrainSet.Count;  // in SampleSet? // changes after injection (if not put in first if clause)?
            });
        }

        #region helpers

        private static void SetAppendedSamplesPerLabel(SampleSet set, decimal injSetFraction, Dictionary<string, int> testResult)
        {
            int totalUnrecognizedSamples = testResult.Values.Sum();

            foreach (var item in testResult)
            {
                decimal fractionOfAllUnrecognizedSamples = totalUnrecognizedSamples == 0
                ? 0
                : item.Value / totalUnrecognizedSamples;
                set.AppendedSamplesPerLabel[item.Key] = (int)(set.SamplesTotal * injSetFraction * fractionOfAllUnrecognizedSamples);
            }
        }
        private static IEnumerable<int?> GetRandomIndeces(SampleSet set, IGrouping<string, Sample> group, bool equalizeGroupSizes)
        {
            int minGroupLength = set.GroupedSamples.Min(x => x.Count()),
                groupLength = group.Count(),
                intendedGroupLength = equalizeGroupSizes ? minGroupLength : groupLength;

            var result = Enumerable.Cast<int?>(
                Enumerable.Range(0, groupLength))
                .Shuffle()
                .Take(intendedGroupLength);

            return result;
        }
        private static void SetArrangedTrainSet(SampleSet set)   // , bool shuffleSamples
        {
            set.ArrangedTrainSet.Clear();

            int lengthOfBiggestGroup = set.GroupedAndRandomizedIndeces.Values.Max(x => x.Length);

            for (int i = 0; i < lengthOfBiggestGroup; i++)
            {
                foreach (var group in set.GroupedAndRandomizedIndeces)
                {
                    if (i == group.Value.Length)
                        set.GroupedAndRandomizedIndeces.Remove(group.Key);
                    else
                        // arrangedTrainSet of indeces only??
                        set.ArrangedTrainSet.Add(set.GroupedSamples.First(x => x.Key == group.Key).ElementAt((int)group.Value.NextItem));
                }
            }
        }

        #endregion

    }
}
