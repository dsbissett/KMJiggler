using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using KMJiggler.Annotations;
using WindowsInput;
using WindowsInput.Native;

namespace KMJiggler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly Timer timer;
        private readonly InputSimulator inputSimulator;
        private int mouseMovementAmount;

        public MainWindow()
        {
            this.InitializeComponent();

            this.timer = new Timer(5000);

            this.inputSimulator = new InputSimulator();
        }

        private void OnMouseMovementIntervalChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => this.mouseMovementAmount = Convert.ToInt32(e.NewValue);

        private void OnMouseIntervalTimingChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!((Slider) sender).IsInitialized)
            {
                return;
            }

            this.timer.Stop();
            this.timer.Interval = e.NewValue * 1000;
            this.timer.Start();
        }

        private void OnKeyboardIntervalChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!((Slider)sender).IsInitialized)
            {
                return;
            }

            this.timer.Stop();
            this.timer.Interval = e.NewValue * 1000;
            this.timer.Start();
        }

        private void OnMouseButtonToggleUnchecked(object sender, RoutedEventArgs e)
        {
            this.timer.Stop();
            this.timer.Enabled = false;
        }

        private void OnKeyboardToggleButtonUnchecked(object sender, RoutedEventArgs e)
        {
            this.timer.Stop();
            this.timer.Enabled = false;
        }

        private void OnMouseButtonToggleChecked(object sender, RoutedEventArgs e)
        {
            this.timer.Elapsed += this.OnMouseTimedEvent;
            this.timer.Enabled = true;
            this.timer.Start();
        }

        private void OnKeyboardToggleButtonChecked(object sender, RoutedEventArgs e)
        {
            this.timer.Elapsed += this.OnKeyboardTimedEvent;
            this.timer.Enabled = true;
            this.timer.Start();
        }

        private void OnMouseTimedEvent(object sender, ElapsedEventArgs e) => this.inputSimulator.Mouse.MoveMouseBy(this.mouseMovementAmount, this.mouseMovementAmount);

        private void OnKeyboardTimedEvent(object sender, ElapsedEventArgs e) => this.inputSimulator.Keyboard.KeyPress(VirtualKeyCode.CONTROL);

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
