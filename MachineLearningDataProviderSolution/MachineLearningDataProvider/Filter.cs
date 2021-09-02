using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Text;

namespace MachineLearningDataProvider
{
    public static class Filter
    {
        public static MLContext AppendFilter(this MLContext mlc, IDataView data, string columnName, double lowerBound = -double.NegativeInfinity, double upperBound = double.PositiveInfinity)
        {
            var filteredData = mlc.Data.FilterRowsByColumn(data, columnName, lowerBound, upperBound);
            return mlc;
        }
    }
}
