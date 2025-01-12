using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Desktop.Views
{
    public partial class SettingsPage
    {
        private AppBarWindow _appBarWindow;

        public SettingsPage()
        {
            InitializeComponent();
        }

        public CheckBox ShowSpeedInTrayCheckBox => ShowSpeedInTrayCheckBoxControl;

        private void ShowSpeedInTrayCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _appBarWindow ??= new AppBarWindow();
            _appBarWindow.Show();
        }

        private void ShowSpeedInTrayCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _appBarWindow.Close();
            _appBarWindow = null;
        }
    }
}