using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfLab3.Mvvm
{
    public class AsyncRelayCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool>? _canExecute;
        private bool _isRunning;

        public AsyncRelayCommand(Func<Task> execute) : this(execute, null)
        {
        }

        public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute)
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
            if (_isRunning)
            {
                return false;
            }
            if (_canExecute == null)
            {
                return true;
            }
            return _canExecute();
        }

        public void Execute(object? parameter)
        {
            Task ignored = ExecuteAsync(parameter);
        }

        public async Task ExecuteAsync(object? parameter)
        {
            if (!CanExecute(parameter))
            {
                return;
            }
            _isRunning = true;
            RaiseCanExecuteChanged();
            try
            {
                await _execute();
            }
            finally
            {
                _isRunning = false;
                RaiseCanExecuteChanged();
            }
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

    public class AsyncRelayCommand<T> : ICommand
    {
        private readonly Func<T?, Task> _execute;
        private readonly Func<T?, bool>? _canExecute;
        private bool _isRunning;

        public AsyncRelayCommand(Func<T?, Task> execute) : this(execute, null)
        {
        }

        public AsyncRelayCommand(Func<T?, Task> execute, Func<T?, bool>? canExecute)
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
            if (_isRunning)
            {
                return false;
            }
            T? typed = ConvertParameter(parameter);
            if (_canExecute == null)
            {
                return true;
            }
            return _canExecute(typed);
        }

        public void Execute(object? parameter)
        {
            Task ignored = ExecuteAsync(parameter);
        }

        public async Task ExecuteAsync(object? parameter)
        {
            if (!CanExecute(parameter))
            {
                return;
            }
            T? typed = ConvertParameter(parameter);
            _isRunning = true;
            RaiseCanExecuteChanged();
            try
            {
                await _execute(typed);
            }
            finally
            {
                _isRunning = false;
                RaiseCanExecuteChanged();
            }
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
