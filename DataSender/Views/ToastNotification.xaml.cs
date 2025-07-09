using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DataSender.Views
{
    public partial class ToastNotification : Window
    {
        private readonly DispatcherTimer _closeTimer = new();

        public ToastNotification(string title, string message, bool showSpinner = false)
        {
            InitializeComponent();
            Opacity = 0;
            TitleText.Text = title;
            MessageText.Text = message;
            LoadingBar.Visibility = showSpinner ? Visibility.Visible : Visibility.Collapsed;

            ContentRendered += (s, e) =>
            {
                Console.WriteLine("ContentRendered fired");

                // Position after layout
                var workingArea = SystemParameters.WorkArea;
                Left = workingArea.Right - ActualWidth - 10;
                Top = workingArea.Bottom - ActualHeight - 10;

                // Animate from 0 to 1
                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
                BeginAnimation(Window.OpacityProperty, fadeIn);
            };

            // Timer to close toast
            _closeTimer.Interval = TimeSpan.FromSeconds(3);
            _closeTimer.Tick += (s, e) => CloseToast();
            _closeTimer.Start();
        }


        private void CloseToast()
        {
            _closeTimer.Stop();
            var fadeOut = new DoubleAnimation(Opacity, 0, TimeSpan.FromMilliseconds(300));
            fadeOut.Completed += (s, e) => Close();
            BeginAnimation(OpacityProperty, fadeOut);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _closeTimer.Stop();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) { }
    }

}
