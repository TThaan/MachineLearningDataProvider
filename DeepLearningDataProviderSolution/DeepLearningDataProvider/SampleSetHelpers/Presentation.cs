using System;
using System.Linq;
using System.Text;

namespace DeepLearningDataProvider.SampleSetHelpers
{
    public static class Presentation
    {
        public static string GetPreviewOfSamples(this ISampleSet sampleSet, int lineBreakAfter = 4, int spacesInNewLine = 5)
        {
            if (sampleSet.Samples == null || sampleSet.Samples.Length == 0)
                return "Samples have not been loaded yet.";

            int longestLabelLength = sampleSet.Samples.Max(x => x.Label).Length;

            StringBuilder sb = new StringBuilder();

            sb.Append($"Training Samples : {(sampleSet.TrainSet == null ? 0 : sampleSet.TrainSet.Count())}\n");
            sb.Append($"Test Samples     : {(sampleSet.TestSet == null ? 0 : sampleSet.TestSet.Count())}\n");

            if (!sampleSet.IsInitialized)
            {
                sb.Append("\nSamples (first 5).\n\n");
                for (int i = 0; i <= 5; i++)
                {
                    sb.Append($"{sampleSet.Samples[i].Label}");
                    sb.AddSpacing(longestLabelLength, sampleSet.Samples[i].Label.Length);
                    sb.Append($" : {sampleSet.Samples[i].Features.ToStringFromCollection(", ", lineBreakAfter, spacesInNewLine)}\n");
                }
            }
            else
            {
                sb.Append("\nTraining samples (first 5).\n\n");
                for (int i = 0; i <= 5; i++)
                {
                    sb.Append($"{sampleSet.TrainSet[i].Label}");
                    sb.AddSpacing(longestLabelLength, sampleSet.TrainSet[i].Label.Length);
                    sb.Append($" : {sampleSet.TrainSet[i].Features.ToStringFromCollection(", ", lineBreakAfter, spacesInNewLine)}\n");
                }
            }

            return sb.ToString();
        }
        public static string GetPreviewOfTargets(this ISampleSet sampleSet, int limit)
        {
            if (sampleSet.Samples == null || sampleSet.Samples.Length == 0)
                return "Samples have not been loaded yet.";

            var labels = sampleSet.Samples.Select(x => x.Label).Distinct().ToArray();   // sampleSet.Targets.Keys.ToArray();
            int longestLabelLength = labels.Max().Length;

            StringBuilder sb = new StringBuilder();

            sb.Append($"Labels & targets ({limit} (max) of {labels.Length} (total)).\n\n");
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
