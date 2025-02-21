using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hospital
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (LoginBox.Text == null || LoginBox.Text == "" || PassBox.Text == null || PassBox.Text == "")
                throw new Exception();
        }

        private void LoginBox_GotFocus(object sender, RoutedEventArgs e)
        {
            LoginBox.Text = "";
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            LoginBox.Foreground = brush;
        }

        private void PassBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PassBox.Text = "";
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            PassBox.Foreground = brush;

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Hide();
            Registration regWin = new Registration();
            regWin.Show();
        }
    }
}