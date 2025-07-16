using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DataSender.Views;

namespace DataSender.Services
{
    public static class ToastService
    {
        public static void ShowToast(string title, string message, bool showSpinner = false)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var toast = new ToastNotification(title, message, showSpinner)
                {
                    Topmost = true
                };
                toast.Show();
            });
        }
    }
}
