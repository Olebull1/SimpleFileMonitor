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
using DataSender.Models;

namespace DataSender.Views
{
    public partial class SettingsWindow : Window
    {
        public AppSettings Settings { get; private set; }

        public SettingsWindow(AppSettings currentSettings)
        {
            InitializeComponent();
            Settings = currentSettings;

            WatchDirBox.Text = Settings.WatchDirectory;
            SuffixBox.Text = Settings.FileSuffix;
            DestDirBox.Text = Settings.DestinationDirectory;
            BackupDirBox.Text = Settings.BackupDirectory;
            Console.WriteLine("Settings IsRemovable is: " + Settings.IsRemovable);
            IsRemovableBox.IsChecked = true;
            //IsRemovableBox.IsChecked = Settings.IsRemovable;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Settings.WatchDirectory = WatchDirBox.Text.Trim();
            Settings.FileSuffix = SuffixBox.Text.Trim();
            Settings.DestinationDirectory = DestDirBox.Text.Trim();
            Settings.BackupDirectory = BackupDirBox.Text.Trim();
            Settings.IsRemovable = IsRemovableBox.IsChecked == true;

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
