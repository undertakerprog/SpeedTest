using System.Net.Http;
using System.Windows;
using Desktop.Services;

namespace Desktop.Views
{
    public partial class MainPage
    {
        private readonly SpeedTestService _speedTestService;
        public MainPage()
        {
            InitializeComponent();
            _speedTestService = new SpeedTestService(new HttpClient { BaseAddress = new Uri("http://localhost:5252") });
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FastSpeedTestRadioButton.IsChecked == true)
                {
                    StartButton.Visibility = Visibility.Collapsed;
                    SpeedResultText.Text = "Measuring speed... Please wait.";
                    SpeedResultText.Visibility = Visibility.Visible;

                    var speedResult = await _speedTestService.GetFastDownloadSpeedAsync();

                    SpeedResultText.Text = !string.IsNullOrEmpty(speedResult)
                        ? $"Download Speed: {speedResult}"
                        : "Error fetching speed.";
                }
                else if (SpeedTestRadioButton.IsChecked == true)
                {
                    // TODO
                }
                else
                {
                    MessageBox.Show("Select type for speedtest\n(Fast SpeedTest/SpeedTest)", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            SpeedResultText.Visibility = Visibility.Collapsed;

            StartButton.Visibility = Visibility.Visible;
        }
    }
}
