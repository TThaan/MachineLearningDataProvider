using DeepLearningDataProvider.Builders;
using Microsoft.ML;
using Microsoft.ML.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using static Microsoft.ML.DataOperationsCatalog;

namespace DeepLearningDataProvider
{
    public class Initializer
    {
        #region fields & ctor

        private MLContext mlc;
        private PathBuilder pathBuilder;
        //private SampleSetParameterBuilder parameterBuilder;
        private ISampleSet sampleSet;
        // private IDataView loadedData;

        public Initializer()
        {
            mlc = new MLContext();
            pathBuilder = new PathBuilder(OnDataProviderChanged);                                 // via DC?
            //parameterBuilder = new SampleSetParameterBuilder(pathBuilder, OnDataProviderChanged); // via DC?
        }

        #endregion

        #region properties

        public PathBuilder PathBuilder
        {
            get
            {
                if (pathBuilder == null)
                    OnDataProviderChanged("Paths are null");
                return pathBuilder;
            }
        }
        //public SampleSetParameterBuilder ParameterBuilder
        //{
        //    get
        //    {
        //        if (parameterBuilder == null)
        //            OnDataProviderChanged("ParameterBuilder are null");
        //        return parameterBuilder;
        //    }
        //}
        public ISampleSet SampleSet
        {
            get
            {
                if (sampleSet == null)
                    sampleSet = new SampleSet();
                    //OnDataProviderChanged("SampleSet is null");
                return sampleSet;
            }
            set { sampleSet = value; }
        }
        //public IDataView LoadedData
        //{
        //    get
        //    {
        //        return loadedData;
        //    }
        //    set { loadedData = value; }
        //}

        #endregion

        #region methods

        public async Task<bool> GetSampleSetAsync(string samplesFileName, float testSamplesFraction, int columnIndex_Label, params int[] ignoredColumnIndeces)//, int columnIndex_Target = -1
        {
            return await Task.Run(() =>
            {
                try
                {
                    OnDataProviderChanged("Loading samples from file, please wait...");

                    IEnumerable<Sample> samples = GetRawSamples(samplesFileName, columnIndex_Label, out int vectorSize);

                    int trainSamplesCount = (int)Math.Round(samples.Count() * (1 - testSamplesFraction), 0);
                    SampleSet.TrainSet = samples.Take(trainSamplesCount).ToArray();
                    SampleSet.TestSet = samples.Skip(trainSamplesCount).ToArray();

                    OnDataProviderChanged("Successfully loaded samples.");
                    return true;
                }
                catch (Exception e) { OnDataProviderChanged(e.Message); return false; }
            });
        }
        public async Task<bool> LoadSampleSetViaMLNetAsync(string samplesFileName, float testSamplesFraction, int columnIndex_Label, params int[] ignoredColumnIndeces)//, int columnIndex_Target = -1
        {
            // vgl: https://github.com/dotnet/machinelearning/issues/164 -> Task: Create 'dataArray' from file on your own!
            // vgl: https://stackoverflow.com/a/68047055/10547243
            // vgl: https://stackoverflow.com/a/55738567/10547243
            // vgl: https://stackoverflow.com/a/54407386/10547243

            return await Task.Run(() =>
            {
                try
                {
                    OnDataProviderChanged("Loading samples from file, please wait...");

                    // Get samples "manually"

                    //IEnumerable<string> lines = File.ReadLines(samplesFileName);
                    //int vectorSize = lines.First().Where(x => x == ',').Count();

                    IEnumerable<Sample> samples = GetRawSamples(samplesFileName, columnIndex_Label, out int vectorSize);

                    // or use TextLoader

                    //var textLoader = mlc.Data.CreateTextLoader(new[]
                    //{
                    //    new TextLoader.Column("Label", DataKind.Single, 0),
                    //    new TextLoader.Column("Features", DataKind.Single, 1, vectorSize)
                    //    //, new TextLoader.Column("Target", DataKind.Single, columnIndex_Target)
                    //});

                    #region Redundant (is already or can be done in 'GetRawSamples')?

                    var schemaDef = SchemaDefinition.Create(typeof(Sample));
                    schemaDef["Features"].ColumnType = new VectorDataViewType(NumberDataViewType.Single, vectorSize);

                    IDataView LoadedData = mlc.Data.LoadFromEnumerable(samples);//, schemaDef
                    var debug = LoadedData.Preview();

                    //DataView LoadedData = mlc.Data.LoadFromTextFile<Sample>(
                    //   path: samplesFileName,
                    //   separatorChar: ',');

                    TrainTestData split = mlc.Data.TrainTestSplit(
                        LoadedData,
                        testFraction: testSamplesFraction
                        //, samplingKeyColumnName: "Group"
                        );    // redundant? Check: https://docs.microsoft.com/de-de/dotnet/api/microsoft.ml.dataoperationscatalog.traintestsplit?view=ml-dotnet

                    #endregion

                    SampleSet.TrainSet = mlc.Data.CreateEnumerable<Sample>(split.TrainSet, reuseRowObject: false).ToArray();
                    SampleSet.TestSet = mlc.Data.CreateEnumerable<Sample>(split.TestSet, reuseRowObject: false).ToArray();

                    OnDataProviderChanged("Successfully loaded samples.");
                    return true;
                }
                catch (Exception e) { OnDataProviderChanged(e.Message); return false; }
            });
        }

        private IEnumerable<Sample> GetRawSamples(string samplesFileName, int columnIndex_Label, out int vectorSize)
        {
            IEnumerable<string> lines = File.ReadLines(samplesFileName);
            List<Sample> result = new List<Sample>(lines.Count());

            // Get amount of feature columns

            vectorSize = lines.First().Where(x => x == ',').Count();

            // Get Samples (i.e. convert line to Sample?)
            foreach (var line in lines)
            {
                List<float> features = new List<float>(vectorSize);

                Sample newSample = new Sample { Label = null, Features = new float[vectorSize] };
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
        }

        /// <summary>
        /// Currently only csv is loaded. More is on the tasks list.
        /// </summary>
        /// <param name="testSamplesCount">Amount of samples used for testing instead of training.</param>
        /// <returns></returns>
        public async Task<bool> LoadSampleSetAsync(string samplesFileName, int testSamplesCount)
        {
            try
            {
                OnDataProviderChanged("Loading samples from file, please wait...");

                List<float[][]> samples = new List<float[][]>();
                List<float[][]> targets = new List<float[][]>();

                string samplesString = await ImpEx.Import.LoadAsOriginalFileTextAsync(samplesFileName);    // PathBuilder.SampleSet
                
                //var jsonString = await File.ReadAllTextAsync(PathBuilder.SampleSet);

                //dynamic dynamicSampleSet = JObject.Parse(jsonString);
                //SampleSetParameters parameters = ((JObject)dynamicSampleSet.Parameters).ToObject<SampleSetParameters>();
                //Sample[] testingSamples = ((JArray)dynamicSampleSet.TestingSamples).ToObject<Sample[]>();
                //Sample[] trainingSamples = ((JArray)dynamicSampleSet.TrainingSamples).ToObject<Sample[]>();

                //SampleSet = new SampleSet
                //{
                //    Parameters = parameters,
                //    TestingSamples = testingSamples,
                //    TrainingSamples = trainingSamples
                //};
                //Sample.Tolerance = parameters.TargetTolerance;
                OnDataProviderChanged("Successfully loaded samples.");
                return true;
            }
            catch (Exception e) { OnDataProviderChanged(e.Message); return false; }
        }
        public async Task<bool> SaveSampleSetAsync()
        {
            try
            {
                OnDataProviderChanged("Saving sample set, please wait...");

                var jsonString = JsonConvert.SerializeObject(SampleSet, Formatting.Indented);
                await File.AppendAllTextAsync(PathBuilder.SampleSet, jsonString);

                OnDataProviderChanged("Successfully saved sample set.");
                return true;
            }
            catch (Exception e) { OnDataProviderChanged(e.Message); return false; }
        }

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
