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
        private readonly DesktopSpeedTestService _speedTestService;

        private Server _selectedServer;

        public MainPage()
        {
            InitializeComponent();

            _speedTestService = new DesktopSpeedTestService(new HttpClient { BaseAddress = new Uri("http://localhost:5252") });

            SpeedTestRadioButton.Checked += RadioButton_Checked;
            FastSpeedTestRadioButton.Checked += RadioButton_Checked;

            _ = InitializeBestServerAsync();
            _ = LoadServersAsync();
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
                    ServersComboBox.Visibility = Visibility.Collapsed;

                    var result = await _speedTestService.GetDownloadSpeedAsync(_selectedServer.Host);

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

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ServersComboBox.Visibility = SpeedTestRadioButton.IsChecked == true
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private async Task InitializeBestServerAsync()
        {
            try
            {
                ServersComboBox.Items.Clear();
                ServersComboBox.Items.Add("Loading...");
                ServersComboBox.SelectedIndex = 0;
                ServersComboBox.IsEnabled = false;

                var response = await _httpClient.GetStringAsync("http://localhost:5252/api/location/best-server");
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var responseServer = JsonSerializer.Deserialize<ServerResponse>(response, options);

                if (responseServer?.Server != null)
                {
                    _selectedServer = responseServer.Server;
                    Debug.WriteLine($"Server: {_selectedServer.Host}");

                    UpdateServersComboBox();
                    await LoadServersAsync();
                }
                else
                {
                    MessageBox.Show("Failed to fetch the best server.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading best server: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ServersComboBox.IsEnabled = true;
                ServersComboBox.Items.Remove("Loading...");
            }
        }

        private void UpdateServersComboBox()
        {
            if (_selectedServer == null || string.IsNullOrEmpty(_selectedServer.City) ||
                string.IsNullOrEmpty(_selectedServer.Provider))
                return;

            var selectedDisplayName = $"{_selectedServer.City}-{_selectedServer.Provider}";

            if (!ServersComboBox.Items.Contains(selectedDisplayName))
            {
                ServersComboBox.Items.Add(selectedDisplayName);
            }

            var itemIndex = ServersComboBox.Items.IndexOf(selectedDisplayName);
            if (itemIndex >= 0)
            {
                ServersComboBox.SelectedIndex = itemIndex;
            }
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
                    MessageBox.Show("Server's list is empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                foreach (var server in servers)
                {
                    var serverDisplayName = $"{server.City}-{server.Provider}";
                    if (!ServersComboBox.Items.Contains(serverDisplayName))
                    {
                        ServersComboBox.Items.Add(serverDisplayName);
                    }
                }

                if (_selectedServer != null)
                {
                    UpdateServersComboBox();
                }

                ServersComboBox.SelectionChanged += (sender, e) =>
                {
                    UpdateSelectedServer(servers);
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading servers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateSelectedServer(List<Server> servers)
        {
            var selectedItem = ServersComboBox.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedItem))
                return;
            _selectedServer = servers.FirstOrDefault(s => $"{s.City}-{s.Provider}" == selectedItem);
            Debug.WriteLine($"Selected server: {_selectedServer?.City}-{_selectedServer?.Provider}");
        }
    }
}
