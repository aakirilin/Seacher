using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace Seacher.Common
{
    public class Command : ICommand
    {
        private readonly Func<object, Task> asincAction;
        private readonly Action<object> action;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (asincAction != null)
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(asincAction, parameter);
            }
            else if (action != null)
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(action, parameter);
            }
        }

        public Command() { }

        public Command(Func<object, Task> asincAction)
        {
            this.asincAction = asincAction;
        }

        public Command(Action<object> action)
        {
            this.action = action;
        }
    }
}
