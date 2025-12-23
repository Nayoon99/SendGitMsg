using System.ComponentModel;

namespace SendGitMsg.Model
{
    internal class CommitRecord : INotifyPropertyChanged
    {
        private string _date = string.Empty;
        public string Date
        {
            get => _date;
            set { _date = value; OnPropertyChanged(nameof(Date)); }
        }

        private int _commitCnt = 0;
        public int CommitCnt
        {
            get => _commitCnt;
            set { _commitCnt = value; OnPropertyChanged(nameof(CommitCnt)); }
        }

        private string _smsStts = string.Empty;
        public string SmsStts
        {
            get => _smsStts;
            set { _smsStts = value; OnPropertyChanged(nameof(SmsStts)); }
        }

        private string _slackStts = string.Empty;
        public string SlackStts
        {
            get => _slackStts;
            set { _slackStts = value; OnPropertyChanged(nameof(SlackStts)); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
