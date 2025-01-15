using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Desktop.Views
{
    public partial class AppBarWindow
    {
        private DispatcherTimer _timer;
        private long _previousReceivedBytes;
        private long _previousSentBytes;

        public AppBarWindow()
        {
            InitializeComponent();
            SpeedLabel.Content = "Initialization...";
            Loaded += AppBarWindow_Loaded;
            InitializeNetworkSpeedUpdater();
        }

        private void AppBarWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow == null)
                return;
            var screenWidth = mainWindow.Width;
            var screenHeight = mainWindow.Height;
            var windowWidth = Width;
            var windowHeight = Height;

            Left = (screenWidth - windowWidth) / 2 + mainWindow.Left;
            Top = (screenHeight - windowHeight) / 2 + mainWindow.Top;
        }

        private void InitializeNetworkSpeedUpdater()
        {
            var (receivedBytes, sentBytes) = GetNetworkStatistics();
            _previousReceivedBytes = receivedBytes;
            _previousSentBytes = sentBytes;

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += UpdateNetworkSpeed;
            _timer.Start();
        }

        private void UpdateNetworkSpeed(object sender, EventArgs e)
        {
            try
            {
                var (totalReceivedBytes, totalSentBytes) = GetNetworkStatistics();

                var receivedBytesDelta = totalReceivedBytes - _previousReceivedBytes;
                var sentBytesDelta = totalSentBytes - _previousSentBytes;

                _previousReceivedBytes = totalReceivedBytes;
                _previousSentBytes = totalSentBytes;

                var receivedMbps = receivedBytesDelta * 8 / 1_000_000.0;
                var sentMbps = sentBytesDelta * 8 / 1_000_000.0;

                SpeedLabel.Content = $"Download: {receivedMbps:F2} Mbps\nUpload: {sentMbps:F2} Mbps";
            }
            catch (Exception ex)
            {
                SpeedLabel.Content = $"Error: {ex.Message}";
            }
        }

        private (long receivedBytes, long sentBytes) GetNetworkStatistics()
        {
            try
            {
                var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(nic =>
                        nic.OperationalStatus == OperationalStatus.Up &&
                        (nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                         nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet))
                    .ToList();

                if (!networkInterfaces.Any())
                {
                    SpeedLabel.Content = "No active network interfaces found";
                    return (0, 0);
                }

                long totalReceivedBytes = 0;
                long totalSentBytes = 0;

                foreach (var stats in networkInterfaces.Select(nic => nic.GetIPv4Statistics()))
                {
                    totalReceivedBytes += stats.BytesReceived;
                    totalSentBytes += stats.BytesSent;
                }

                return (totalReceivedBytes, totalSentBytes);
            }
            catch (Exception ex)
            {
                SpeedLabel.Content = $"Error: {ex.Message}";
                return (0, 0);
            }
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _timer?.Stop();
        }
    }
}
