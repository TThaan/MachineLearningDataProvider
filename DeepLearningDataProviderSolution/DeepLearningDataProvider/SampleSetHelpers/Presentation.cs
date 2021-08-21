using System;
using System.Linq;
using System.Text;

namespace DeepLearningDataProvider.SampleSetHelpers
{
    public static class Presentation
    {
        public static string GetPreviewOfSamples(this ISampleSet sampleSet, int lineBreakAfter = 4, int spacesInNewLine = 5)
        {
            if (sampleSet == null || sampleSet.TrainSet == null || sampleSet.TestSet == null)
                return "Sampleset has not been loaded yet.";

            // throw new System.ArgumentException($"Presentation: {sampleSet.Samples.Length}");
            StringBuilder sb = new StringBuilder();

            sb.Append($"Training Samples : {sampleSet.TrainSet.Count()}\n");
            sb.Append($"Test Samples     : {sampleSet.TestSet.Count()}\n");
            sb.Append($"Labels / Targets : {sampleSet.Targets.Count()}\n");

            int longestLabelLength = sampleSet.Targets.Keys.Max().Length;

            sb.Append("\nTraining samples (first 5).\n\n");
            for (int i = 0; i <= 5; i++)
            {
                sb.Append($"{sampleSet.TrainSet[i].Label}");
                sb.AddSpacing(longestLabelLength, sampleSet.TrainSet[i].Label.Length);
                sb.Append($" : {sampleSet.TrainSet[i].Features.ToStringFromCollection(", ", lineBreakAfter, spacesInNewLine)}\n");
            }

            return sb.ToString();
        }
        public static string GetPreviewOfTargets(this ISampleSet sampleSet, int limit)
        {
            if (sampleSet == null || sampleSet.TrainSet == null || sampleSet.TestSet == null)
                return "Sampleset has not been loaded yet.";

            StringBuilder sb = new StringBuilder();

            var labels = sampleSet.Targets.Keys.ToArray();
            int longestLabelLength = labels.Max().Length;

            sb.Append($"Labels & targets ({limit} max).\n\n");
            foreach (var kvp in sampleSet.Targets)
            {
                int index = Array.IndexOf(labels, kvp.Key);
                if (index >= limit)
                    break;

                sb.Append($"Label: {kvp.Key}");
                sb.AddSpacing(longestLabelLength, kvp.Key.Length);
                sb.Append($" Target: {kvp.Value.ToStringFromCollection(", ", 20, 5)}\n");
            }

            return sb.ToString();
        }

        #region helpers

        private static void AddSpacing(this StringBuilder sb, int longestLabelLength, int currentLabelLength)
        {
            int spacing = longestLabelLength - currentLabelLength;

            for (int i = 0; i <= spacing; i++)
                sb.Append(' ');
        }

        #endregion
    }
}
