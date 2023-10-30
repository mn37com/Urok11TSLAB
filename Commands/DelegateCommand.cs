using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Urok11TSLAB.Commands
{
    public class DelegateCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public DelegateCommand(DelegateFunction function) 
        {
            _function=function;
        }
        public delegate void DelegateFunction (object obj);
        public bool CanExecute(object parameter)
        {
            return true;
        }
        // =================================================================
        private DelegateFunction _function;
        public void Execute(object parameter)
        {
            _function?.Invoke(parameter);
        }

    }
}
