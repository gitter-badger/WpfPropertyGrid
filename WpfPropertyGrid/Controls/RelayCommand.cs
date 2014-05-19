namespace WpfPropertyGrid.Vm
{
    using System;
    using System.Windows.Input;

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _action;
        private readonly Predicate<object> _condition;

        public RelayCommand(Action<object> action, Predicate<object> condition)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            _action = action;
            _condition = condition ?? (o => true);
        }

        public RelayCommand(Action<object> action)
            : this(action, o => true)
        {
        }

        /// <summary>
        /// http://stackoverflow.com/a/2588145/1069200
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            return _condition(parameter);
        }

        public void Execute(object parameter)
        {
            _action(parameter);
        }
    }
}