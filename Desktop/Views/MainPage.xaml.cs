using System.Net.Http;
using System.Text.Json;
using System.Windows;
using Desktop.Services;
using Desktop.Model;
using System.Diagnostics;

namespace Desktop.Views
{
    public partial class MainPage
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly SpeedTestService _speedTestService;
        private Server _selectedServer;

        public MainPage()
        {
            InitializeComponent();
            _speedTestService = new SpeedTestService(new HttpClient { BaseAddress = new Uri("http://localhost:5252") });

            SpeedTestRadioButton.Checked += RadioButton_Checked;
            FastSpeedTestRadioButton.Checked += RadioButton_Checked;

            _ = LoadServersAsync();
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
                    MessageBox.Show("Select type for speedtest\n(Fast SpeedTest/SpeedTest)", "Information",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ServersComboBox.Visibility = SpeedTestRadioButton.IsChecked == true
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private async Task LoadServersAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync("http://localhost:5252/api/location/servers-city-list");

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var servers = JsonSerializer.Deserialize<List<Server>>(response, options);

                if (servers == null || servers.Count == 0)
                {
                    MessageBox.Show("Список серверов пуст.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ServersComboBox.Items.Clear();
                foreach (var server in servers)
                {
                    ServersComboBox.Items.Add($"{server.City}-{server.Provider}");
                }

                ServersComboBox.SelectionChanged += (sender, e) =>
                {
                    UpdateSelectedServer(servers);
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке серверов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateSelectedServer(List<Server> servers)
        {
            var selectedItem = ServersComboBox.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedItem)) return;
            _selectedServer = servers.FirstOrDefault(s => $"{s.City}-{s.Provider}" == selectedItem);
            Debug.WriteLine($"Selected server: {_selectedServer?.City}-{_selectedServer?.Provider}");
        }
    }
}
