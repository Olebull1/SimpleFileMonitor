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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DataSender.Views
{
    public partial class RenameDialog : Window
    {
        public string EnteredName { get; private set; } = "";
        public RenameDialog(string suggestedName)
        {
            InitializeComponent();
            NameBox.Text = suggestedName;

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Force this window to the front + Focus Dialog + Select Text
            this.Topmost = true;
            this.Activate();
            this.Focus();

            NameBox.Focus();
            NameBox.SelectAll();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            EnteredName = NameBox.Text.Trim();
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
