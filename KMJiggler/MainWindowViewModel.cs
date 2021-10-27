using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KMJiggler
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> action;

        public RelayCommand(Action<object> action)
        {
            this.action = action;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            if(parameter != null)
            {
                this.action(parameter);
            }
        }
    }

    public class MainWindowViewModel
    {
        private ICommand mouseButtonToggleCommand;

        public MainWindowViewModel()
        {
            //this.MouseButtonToggleCommand = new RelayCommand(new Action<object>())
        }

        public ICommand MouseButtonToggleCommand
        {
            get => this.mouseButtonToggleCommand;
            set => this.mouseButtonToggleCommand = value;
        }
    }
}
