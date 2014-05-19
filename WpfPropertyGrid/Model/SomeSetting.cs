namespace WpfPropertyGrid
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Annotations;

    public class SomeSetting : INotifyPropertyChanged
    {
        private string _stringValue1 ="Random";
        private string _stringValue2 = "Values";
        private string _stringValue3;
        private string _stringValue4;
        private bool _boolValue1;
        private bool _boolValue2;
        private bool _boolValue3;
        private bool _boolValue4;
        public event PropertyChangedEventHandler PropertyChanged;

        public string StringValue1
        {
            get { return _stringValue1; }
            set
            {
                if (value == _stringValue1) return;
                _stringValue1 = value;
                OnPropertyChanged();
            }
        }

        public string StringValue2
        {
            get { return _stringValue2; }
            set
            {
                if (value == _stringValue2) return;
                _stringValue2 = value;
                OnPropertyChanged();
            }
        }

        public string StringValue3
        {
            get { return _stringValue3; }
            set
            {
                if (value == _stringValue3) return;
                _stringValue3 = value;
                OnPropertyChanged();
            }
        }

        public string StringValue4
        {
            get { return _stringValue4; }
            set
            {
                if (value == _stringValue4) return;
                _stringValue4 = value;
                OnPropertyChanged();
            }
        }

        public bool BoolValue1
        {
            get { return _boolValue1; }
            set
            {
                if (value.Equals(_boolValue1)) return;
                _boolValue1 = value;
                OnPropertyChanged();
            }
        }

        public bool BoolValue2
        {
            get { return _boolValue2; }
            set
            {
                if (value.Equals(_boolValue2)) return;
                _boolValue2 = value;
                OnPropertyChanged();
            }
        }

        public bool BoolValue3
        {
            get { return _boolValue3; }
            set
            {
                if (value.Equals(_boolValue3)) return;
                _boolValue3 = value;
                OnPropertyChanged();
            }
        }

        public bool BoolValue4
        {
            get { return _boolValue4; }
            set
            {
                if (value.Equals(_boolValue4)) return;
                _boolValue4 = value;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
