using System;
using System.Windows.Input;

namespace TouchbarMaker.ViewModels
{
    public class Commander : ICommand
    {
        public bool CanExecute(object parameter)
        {
            var result = _canExecuteAction.Invoke(parameter);
            if (_canExecute != result)
            {
                _canExecute = result;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }

            return _canExecute;
        }

        public void Execute(object parameter)
        {
            _execute.Invoke(parameter);
        }

        public event EventHandler CanExecuteChanged;

        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecuteAction;
        private bool _canExecute;

        public Commander(Action<object> execute, Func<object, bool> canExecuteAction)
        {
            _execute = execute;
            _canExecuteAction = canExecuteAction;
        }

        public Commander(Action<object> execute)
        {
            _execute = execute;
            _canExecuteAction = o => true;
        }
    }
}