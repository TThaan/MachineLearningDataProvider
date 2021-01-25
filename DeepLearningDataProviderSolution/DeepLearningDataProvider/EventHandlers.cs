using System;

namespace DeepLearningDataProvider
{
    public delegate void StatusChangedEventHandler(object sender, StatusChangedEventArgs e);
    public class StatusChangedEventArgs : EventArgs
    {
        #region fields & ctor

        private string _info;

        public StatusChangedEventArgs(string info)
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