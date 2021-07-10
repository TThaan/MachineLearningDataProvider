using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;

namespace DeepLearningDataProvider
{
    public static class Transform
    {
        public static ValueToKeyMappingEstimator GetValueToKeyMapper(this MLContext mlc, string outputColumnName, string inputColumnName = null)
        {
            if (inputColumnName == null)
                return mlc.Transforms.Conversion.MapValueToKey(outputColumnName);
            else
                return mlc.Transforms.Conversion.MapValueToKey(outputColumnName, inputColumnName);
        }
        public static EstimatorChain<ValueToKeyMappingTransformer> AppendValueToKeyMapper<TLastTransformer>(this IEstimator<TLastTransformer> estimator, MLContext mlc, string outputColumnName, string inputColumnName = null) where TLastTransformer : class, ITransformer
        {
            if (inputColumnName == null)
                return estimator.Append(mlc.Transforms.Conversion.MapValueToKey(outputColumnName));
            else
                return estimator.Append(mlc.Transforms.Conversion.MapValueToKey(outputColumnName, inputColumnName));
        }


        public static NormalizingEstimator GetNormalizer(this MLContext mlc, string outputColumnName, string inputColumnName = null, bool fixZero = true)
        {
            if (inputColumnName == null)
                return mlc.Transforms.NormalizeMinMax(outputColumnName, fixZero: fixZero);
            else
                return mlc.Transforms.NormalizeMinMax(outputColumnName, inputColumnName, fixZero: fixZero);
        }
        public static EstimatorChain<NormalizingTransformer> AppendNormalizer<TLastTransformer>(this IEstimator<TLastTransformer> estimator, MLContext mlc, string outputColumnName, string inputColumnName = null, bool fixZero = true) where TLastTransformer : class, ITransformer
        {
            if (inputColumnName == null)
                return estimator.Append(mlc.Transforms.NormalizeMinMax(outputColumnName, fixZero: fixZero));
            else
                return estimator.Append(mlc.Transforms.NormalizeMinMax(outputColumnName, inputColumnName, fixZero: fixZero));
        }
    }
}