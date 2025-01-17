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

            var savedUnit = Properties.Settings.Default.SpeedUnit;
            SetSpeedUnitRadioButton(savedUnit);
        }

        public CheckBox ShowSpeedCheckBox => ShowSpeedCheckBoxControl;

        private void ShowSpeedInTrayCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _appBarWindow = new AppBarWindow();
            _appBarWindow.Show();
        }

        private void ShowSpeedInTrayCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _appBarWindow.Close();
            _appBarWindow = null;
        }

        private void SpeedUnitRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is not RadioButton radioButton) return;
            Properties.Settings.Default.SpeedUnit = radioButton.Content.ToString();
            Properties.Settings.Default.Save();
        }

        private void SetSpeedUnitRadioButton(string unit)
        {
            switch (unit)
            {
                case "Kbps":
                    KbpsRadioButton.IsChecked = true;
                    break;
                case "MBps":
                    MBpsRadioButton.IsChecked = true;
                    break;
                case "KBps":
                    KBpsRadioButton.IsChecked = true;
                    break;
                default:
                    MbpsRadioButton.IsChecked = true;
                    break;
            }
        }
    }
}