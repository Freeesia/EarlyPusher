using System;
using System.Windows.Input;

namespace SFLibs.Commands
{
    public class DelegateCommand : ICommand
    {
        private Action<object> execute;
        private Predicate<object> canExecute;


        public DelegateCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public virtual bool CanExecute(object parameter)
        {
            return this.canExecute?.Invoke(parameter) ?? true;
        }

        public event EventHandler CanExecuteChanged;

        public virtual void Execute(object parameter)
        {
            this.execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
