using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Desktop.Model;
using Desktop.Service;

namespace Desktop.Views
{
    public partial class ServerSelectionWindow
    {
        private List<Server>? _servers;
        private readonly DesktopLocationService _locationService;

        public ServerSelectionWindow(List<Server>? servers)
        {
            InitializeComponent();
            _locationService = new DesktopLocationService(new HttpClient());
            _servers = servers;
            LoadServers();
        }

        private void LoadServers()
        {
            ServerLinksPanel.Children.Clear();

            if (_servers == null || _servers.Count == 0)
            {
                MessageBox.Show("The server list is empty or not loaded");
                return;
            }

            foreach (var server in _servers)
            {
                var hyperlink = new Hyperlink(new Run($"{server.Provider} - {server.City}"))
                {
                    NavigateUri = new Uri($"http://{server.Host}")
                };
                hyperlink.Click += Hyperlink_Click;
                hyperlink.Tag = server;

                var textBlock = new TextBlock { Margin = new Thickness(5) };
                textBlock.Inlines.Add(hyperlink);

                ServerLinksPanel.Children.Add(textBlock);
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Hyperlink { Tag: Server selectedServer }) return;
            if (Owner is MainWindow mainWindow &&
                mainWindow.MainFrame.Content is MainPage mainPage)
            {
                mainPage.SetSelectedServer(selectedServer);
            }
            Close();
        }

        private void ServerFilterTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox?.Text != "Search city for test") return;
            textBox.Text = "";
            textBox.Foreground = System.Windows.Media.Brushes.Black;
        }

        private void ServerFilterTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (!string.IsNullOrWhiteSpace(textBox?.Text)) return;
            textBox!.Text = "Search city for test";
            textBox.Foreground = System.Windows.Media.Brushes.Gray;
        }

        private async void ServerFilterTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key != Key.Enter)
                    return;
                e.Handled = true;

                var filterText = ServerFilterTextBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(filterText))
                {
                    MessageBox.Show("Please enter a valid search term.");
                    return;
                }

                ServerLinksPanel.Children.Clear();
                ServerLinksPanel.Children.Add(new TextBlock { Text = "Loading servers...", Foreground = System.Windows.Media.Brushes.Gray });

                try
                {
                    var newServers = await _locationService.GetServersOfCityAsync(filterText);

                    _servers = newServers;

                    LoadServers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading servers: {ex.Message}");
                    LoadServers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}