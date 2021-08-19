using System;
using System.Linq;
using System.Text;

namespace DeepLearningDataProvider.SampleSetExtensions
{
    public static class Presentation
    {
        public static string GetPreviewOfSamples(this ISampleSet sampleSet, int lineBreakAfter = 4, int spacesInNewLine = 5)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"Training Samples : {sampleSet.TrainSet.Count()}\n");
            sb.Append($"Test Samples     : {sampleSet.TestSet.Count()}\n");
            sb.Append($"Labels / Targets : {sampleSet.Targets.Count()}\n");

            sb.Append("\nTraining samples (first 5).\n\n");
            for (int i = 0; i <= 5; i++)
            {
                sb.Append($"Label    : {sampleSet.TrainSet[i].Label}");
                sb.Append($"Features : {sampleSet.TrainSet[i].Features.ToStringFromCollection(", ", lineBreakAfter, spacesInNewLine)}\n");
            }

            return sb.ToString();
        }
        public static string GetPreviewOfTargets(this ISampleSet sampleSet, int limit)
        {
            StringBuilder sb = new StringBuilder();

            var labels = sampleSet.Targets.Keys.ToArray();

            sb.Append($"Labels & targets ({limit} max).\n\n");
            foreach (var kvp in sampleSet.Targets)
            {
                sb.Append($"Label   : {kvp.Key}");
                sb.Append($"Target  : {kvp.Value.ToStringFromCollection(", ", 20, 5)}\n");

                int index = Array.IndexOf(labels, kvp.Key);
                if (index >= limit)
                    break;
            }

            return sb.ToString();
        }
    }
}
