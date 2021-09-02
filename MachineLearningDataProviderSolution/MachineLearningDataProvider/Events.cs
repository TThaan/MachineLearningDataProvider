using System;

namespace MachineLearningDataProvider
{
    public delegate void DataProviderChangedEventHandler(object sender, DataProviderChangedEventArgs e);

    public class DataProviderChangedEventArgs : EventArgs
    {
        #region fields & ctor

        private string _info;

        public DataProviderChangedEventArgs(string info)
        {
            _info = info;
        }

        #endregion

        #region public

        public string Info
        {
            get { return _info; }
        }

        #endregion
    }
}