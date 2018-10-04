using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace InstrumentMonitor
{
    /// <summary>
    /// A generic command implementation.
    /// </summary>
    public class RelayCommand: ICommand
    {
        private readonly Action<object> myExecute;
        private readonly Predicate<object> myCanExecute;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="execute">execution action</param>
        /// <param name="canExecute">Is it possible to execute</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            myExecute = execute ?? throw new ArgumentNullException(nameof(execute));
            myCanExecute = canExecute;
        }

        public RelayCommand(Action<object> execute) : this(execute, null)
        {
        }


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
            return myCanExecute == null ? true : myCanExecute(parameter);
        }

        public void Execute(object parameter)
        {
            myExecute(parameter);
        }
    }
}
