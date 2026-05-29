using System;
using System.Windows.Input;

namespace WpfLab3.Mvvm
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute) : this(execute, null)
        {
        }

        public RelayCommand(Action execute, Func<bool>? canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }
            return _canExecute();
        }

        public void Execute(object? parameter)
        {
            _execute();
        }

        public void RaiseCanExecuteChanged()
        {
            EventHandler? handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T?> _execute;
        private readonly Func<T?, bool>? _canExecute;

        public RelayCommand(Action<T?> execute) : this(execute, null)
        {
        }

        public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            T? typed = ConvertParameter(parameter);
            if (_canExecute == null)
            {
                return true;
            }
            return _canExecute(typed);
        }

        public void Execute(object? parameter)
        {
            T? typed = ConvertParameter(parameter);
            _execute(typed);
        }

        public void RaiseCanExecuteChanged()
        {
            EventHandler? handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private static T? ConvertParameter(object? parameter)
        {
            if (parameter == null)
            {
                return default(T);
            }
            if (parameter is T)
            {
                return (T)parameter;
            }
            return default(T);
        }
    }
}
