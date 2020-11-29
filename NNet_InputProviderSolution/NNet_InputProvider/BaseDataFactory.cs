using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NNet_InputProvider
{
    public abstract class BaseDataFactory
    {
        #region ctor

        /// <summary>
        /// Order:
        /// Url_TrainingLabels, string Url_TrainingImages, string Url_TestingLabels, string Url_TestingImages
        /// </summary>
        public BaseDataFactory(params string[] urls)
        {
            // Try to get samples from local files
            if (File.Exists(urls[0]) && File.Exists(urls[1]) && File.Exists(urls[2]) && File.Exists(urls[3]))
            {
                
            }
            // Try to get samples from web
            else
            {
                
            }
        }
        //public BaseDataFactory()
        //{
        //    // Url_TrainLabels = Url_TrainLabels_Default;
        //    // Url_TrainImages = Url_TrainImages_Default;
        //    // Url_TestLabels = Url_TestLabels_Default;
        //    // Url_TestImages = Url_TestImages_Default;
        //}

        #endregion

        #region public

        // public abstract string Url_TrainImages_Default { get; protected set; }
        // public abstract string Url_TrainLabels_Default { get; protected set; }
        // public abstract string Url_TestImages_Default { get; protected set; }
        // public abstract string Url_TestLabels_Default { get; protected set; }

        public string Url_TrainImages { get; protected set; }
        public string Url_TrainLabels { get; protected set; }
        public string Url_TestImages { get; protected set; }
        public string Url_TestLabels { get; protected set; }

        public Sample[] TrainingSamples { get; protected set; }
        public Sample[] TestingSamples { get; protected set; }

        #endregion
    }
}
