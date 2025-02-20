using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Desktop.Model;

namespace Desktop.Views
{
    public partial class ServerSelectionWindow
    {
        private readonly List<Server>? _servers;

        public ServerSelectionWindow(List<Server>? servers)
        {
            InitializeComponent();
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
            if (textBox?.Text != "Search server") return;
            textBox.Text = "";
            textBox.Foreground = System.Windows.Media.Brushes.Black;
        }

        private void ServerFilterTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (!string.IsNullOrWhiteSpace(textBox?.Text)) return;
            textBox!.Text = "Search server";
            textBox.Foreground = System.Windows.Media.Brushes.Gray;
        }
    }
}