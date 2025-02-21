using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Hospital
{
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

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
            MainWindow main = new();
            main.Show();
        }

        private void NameBox_GotFocus(object sender, RoutedEventArgs e)
        {
            NameBox.Text = "";
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            NameBox.Foreground = brush;
        }

        private void SecondNameBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SecondNameBox.Text = "";
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            SecondNameBox.Foreground = brush;
        }

        private void FatherNameBox_GotFocus(object sender, RoutedEventArgs e)
        {
            FatherNameBox.Text = "";
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            FatherNameBox.Foreground = brush;
        }

        private void PassportBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PassportBox.Text = "";
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            PassportBox.Foreground = brush;
        }

        private void AdressBox_GotFocus(object sender, RoutedEventArgs e)
        {
            AdressBox.Text = "";
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            AdressBox.Foreground = brush;
        }

        private void PhoneBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PhoneBox.Text = "";
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            PhoneBox.Foreground = brush;
        }

        private void EmailBox_GotFocus(object sender, RoutedEventArgs e)
        {
            EmailBox.Text = "";
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            EmailBox.Foreground = brush;
        }

        private void PolicyNumberBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PolicyNumberBox.Text = "";
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            PolicyNumberBox.Foreground = brush;
        }
    }
}
