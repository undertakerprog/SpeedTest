using System.Net.Http;
using System.Text.Json;
using System.Windows;
using Desktop.Services;
using Desktop.Model;
using System.Diagnostics;
using System.Windows.Controls;
using Desktop.Service;

namespace Desktop.Views
{
    public partial class MainPage
    {
        private const string SpeedTestUri = "http://localhost:5252/api/SpeedTest/";
        private const string LocationUri = "http://localhost:5252/api/Location/";

        private readonly HttpClient _httpClient = new HttpClient();

        private readonly DesktopSpeedTestService _speedTestService;
        private readonly DesktopLocationService _locationService;

        public MainPage()
        {
            InitializeComponent();

            _speedTestService = new DesktopSpeedTestService(new HttpClient { BaseAddress = new Uri(SpeedTestUri) });
            _locationService = new DesktopLocationService(new HttpClient { BaseAddress = new Uri(LocationUri) });
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SpeedTestRadioButton.Visibility = Visibility.Collapsed;
                FastSpeedTestRadioButton.Visibility = Visibility.Collapsed;

                StartButton.Visibility = Visibility.Collapsed;
                SpeedResultText.Text = "Measuring speed... Please wait.";
                SpeedResultText.Visibility = Visibility.Visible;

                if (FastSpeedTestRadioButton.IsChecked == true)
                {
                    var speedResult = await _speedTestService.GetFastDownloadSpeedAsync();

                    SpeedResultText.Text = !string.IsNullOrEmpty(speedResult)
                        ? $"Download Speed: {speedResult}"
                        : "Error fetching speed.";
                }
                else if (SpeedTestRadioButton.IsChecked == true)
                {
                    var result = await _speedTestService.GetDownloadSpeedAsync("speedtest.datahata.by");

                    SpeedResultText.Text = !string.IsNullOrEmpty(result)
                        ? $"{result}"
                        : "Error fetching speed.";
                }
                else
                {
                    MessageBox.Show("Select type for speedtest\n(Fast SpeedTest/SpeedTest)", "Information",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void SelectServerLink_Click(object sender, RoutedEventArgs e)
        {
            var serverSelectionWindow = new ServerSelectionWindow
            {
                Owner = Window.GetWindow(this)
            };
            serverSelectionWindow.ShowDialog();
        }
    }
}
