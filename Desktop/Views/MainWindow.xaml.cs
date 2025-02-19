using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Desktop.Views
{
    public partial class MainWindow
    {
        private readonly MainPage _mainPage = new MainPage();
        private readonly SettingsPage _settingsPage = new SettingsPage();

        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new MainPage());
            Closing += MainWindow_Closing!;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content is not MainPage)
            {
                if (MainFrame.Content is SettingsPage)
                {
                    NavigateToPage(_mainPage, "SpeedTest", "../Resources/settings-50.png");
                }
            }
            else
            {
                NavigateToPage(_settingsPage, "Settings", "../Resources/home-50.png");
            }
        }

        private void NavigateToPage(Page page, string pageTitle, string imagePath)
        {
            MainFrame.Navigate(page);
            CurrentPageTitle.Text = pageTitle;

            UpdateUI(pageTitle, imagePath);
        }

        private void UpdateUI(string pageTitle, string imagePath)
        {
            CurrentPageTitle.Text = pageTitle;

            var bitmapImage = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
            CurrentImage.Source = bitmapImage;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            if (_settingsPage.ShowSpeedCheckBox.IsChecked == true)
            {
                _settingsPage.ShowSpeedCheckBox.IsChecked = false;
            }
        }
    }
}