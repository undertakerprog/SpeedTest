using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Desktop.Views
{
    public partial class AppBarWindow
    {
        private DispatcherTimer _timer;
        private PerformanceCounter _bytesReceivedCounter;
        private PerformanceCounter _bytesSentCounter;

        public AppBarWindow()
        {
            InitializeComponent();
            InitializeNetworkSpeedUpdater();
            Loaded += AppBarWindow_Loaded;
        }

        private void AppBarWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow == null) return;
            var screenWidth = mainWindow.Width;
            var screenHeight = mainWindow.Height;
            var windowWidth = Width;
            var windowHeight = Height;

            Left = (screenWidth - windowWidth) / 2 + mainWindow.Left;
            Top = (screenHeight - windowHeight) / 2 + mainWindow.Top;
        }

        private void InitializeNetworkSpeedUpdater()
        {
            var category = new PerformanceCounterCategory("Network Interface");
            var instanceNames = category.GetInstanceNames();

            if (instanceNames.Length > 0)
            {
                _bytesReceivedCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", instanceNames[0]);
                _bytesSentCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", instanceNames[0]);
            }
            else
            {
                throw new InvalidOperationException("No network interfaces found.");
            }

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += UpdateNetworkSpeed!;
            _timer.Start();
        }

        private void UpdateNetworkSpeed(object sender, EventArgs e)
        {
            try
            {
                var bytesReceivedPerSecond = _bytesReceivedCounter.NextValue();
                var bytesSentPerSecond = _bytesSentCounter.NextValue();

                var receivedMbps = bytesReceivedPerSecond * 8 / 1_000_000;
                var sentMbps = bytesSentPerSecond * 8 / 1_000_000;

                SpeedLabel.Content = $"Download: {receivedMbps:F2} Mbps\nUpload: {sentMbps:F2} Mbps";
            }
            catch
            {
                SpeedLabel.Content = "Error retrieving network speed";
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
            _timer.Stop();
            _bytesReceivedCounter.Dispose();
            _bytesSentCounter.Dispose();
        }
    }
}
