namespace WpfPropertyGrid.Vms
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;
    using Annotations;
    using Vm;

    public abstract class SettingsViewModelBase<TSetting> : INotifyPropertyChanged, ISaveUndo where TSetting : INotifyPropertyChanged
    {
        private readonly IRepository _repository;
        private readonly Action<IRepository, TSetting> _saveAction;
        private readonly string _name;
        private readonly UndoCache<TSetting> _cache = new UndoCache<TSetting>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="setting"></param>
        /// <param name="saveAction">(repo, velocitySettings) => repo.SaveAsync(velocitySettings) how to save the setting</param>
        /// <param name="name">Name is provided if two settings of the same type are going to be cached</param>
        protected SettingsViewModelBase(IRepository repository, TSetting setting, Action<IRepository, TSetting> saveAction, string name = null)
        {
            _repository = repository;
            Setting = setting;
            _cache.Cache(setting, name);
            _saveAction = saveAction;
            _name = name;
            SaveCommand = new RelayCommand(_ => Save(), _ => CanSave());
            UndoCommand = new RelayCommand(_ => Undo(), _ => CanUndo());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public TSetting Setting { get; private set; }

        public TSetting CachedSetting
        {
            get
            {
                return _cache.CachedValue;
            }
        }

        public ICommand SaveCommand { get; protected set; }

        public ICommand UndoCommand { get; protected set; }

        public bool CanSave()
        {
            return _cache.IsDirty(Setting, _name);
        }

        public virtual void Save()
        {
            _saveAction(_repository, Setting);
            _cache.Cache(Setting, _name);
            OnPropertyChanged("CachedSetting");
        }

        public bool CanUndo()
        {
            return _cache.IsDirty(Setting, _name);
        }

        public virtual void Undo()
        {
            _cache.Reset(Setting, _name);
        }

        public override string ToString()
        {
            return string.Format("Setting: {0} Name: {1}, CanSave: {2}, CanUndo: {3}", Setting, _name, CanSave(), CanUndo());
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}