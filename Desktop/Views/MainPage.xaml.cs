using System.Net.Http;
using System.Windows;
using Desktop.Services;
using Desktop.Model;
using Desktop.Service;
using System.Windows.Documents;
using System.Windows.Media;

namespace Desktop.Views
{
    public partial class MainPage
    {
        private readonly HttpClient _httpClient = new HttpClient();

        private readonly DesktopSpeedTestService _speedTestService;
        private readonly DesktopLocationService _locationService;

        private Server? _bestServer;
        private List<Server> _servers;
        private Server? _selectedServer;

        public MainPage()
        {
            InitializeComponent();

            _speedTestService = new DesktopSpeedTestService(new HttpClient());
            _locationService = new DesktopLocationService(new HttpClient());

            LoadServerForTest();
            LoadListServerOfUserCity();
        }

        public void SetSelectedServer(Server server)
        {
            _selectedServer = server;
            LoadServerForTest();
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
                    ServerTextBlock.Visibility = Visibility.Collapsed;

                    CollapsedSelectServerLink();

                    string result;
                    if (_selectedServer == null)
                    {
                        result = await _speedTestService.GetDownloadSpeedAsync(_bestServer!.Host);
                    }
                    else
                    {
                        result = await _speedTestService.GetDownloadSpeedAsync(_selectedServer!.Host);
                    }

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

        private async void LoadListServerOfUserCity()
        {
            try
            {
                var servers = await _locationService.GetServersOfCityAsync("Minsk");
                if (servers != null)
                {
                    _servers = servers;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void LoadServerForTest()
        {
            try
            {
                ServerTextBlock.Inlines.Clear();

                if (_selectedServer != null)
                {
                    ServerTextBlock.Inlines.Add(new Run
                    {
                        Text = _selectedServer.Provider + "\n",
                        FontSize = 18
                    });

                    ServerTextBlock.Inlines.Add(new Run
                    {
                        Text = _selectedServer.City,
                        FontSize = 14,
                        Foreground = new SolidColorBrush(Color.FromArgb(204, 0, 0, 0))
                    });

                    VisibleSelectServerLink();
                    return;
                }

                ServerTextBlock.Text = "Loading...";
                CollapsedSelectServerLink();

                _bestServer = await _locationService.GetBestServerAsync();

                if (_bestServer != null)
                {
                    ServerTextBlock.Inlines.Clear();

                    ServerTextBlock.Inlines.Add(new Run
                    {
                        Text = _bestServer.Provider + "\n",
                        FontSize = 18
                    });

                    ServerTextBlock.Inlines.Add(new Run
                    {
                        Text = _bestServer.City,
                        FontSize = 14,
                        Foreground = new SolidColorBrush(Color.FromArgb(204, 0, 0, 0))
                    });

                    VisibleSelectServerLink();
                }
                else
                {
                    ServerTextBlock.Text = "Failed to load server";
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FastSpeedTestRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ServerTextBlock.Visibility = Visibility.Collapsed;
            CollapsedSelectServerLink();
        }

        private void FastSpeedTestRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ServerTextBlock.Visibility = Visibility.Visible;
            VisibleSelectServerLink();
        }

        private void SelectServerLink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var serverSelectionWindow = new ServerSelectionWindow(_servers)
                {
                    Owner = Window.GetWindow(this)
                };
                serverSelectionWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CollapsedSelectServerLink()
        {
            SelectServerLink.Foreground = new SolidColorBrush(Colors.Transparent);
            SelectServerLink.IsEnabled = false;
        }

        private void VisibleSelectServerLink()
        {
            SelectServerLink.Foreground = new SolidColorBrush(Colors.Blue);
            SelectServerLink.IsEnabled = true;
        }
    }
}
