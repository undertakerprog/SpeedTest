using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Desktop.Views
{
    public partial class ServerSelectionWindow : Window
    {
        public ServerSelectionWindow()
        {
            InitializeComponent();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Hyperlink hyperlink)
            {
                //logic for choose server 
            }
        }

        private void ServerFilterTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //logic for search server of country or city
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