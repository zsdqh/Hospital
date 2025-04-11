using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<string, string> doctors = new Dictionary<string, string>();
        public MainWindow()
        {
            // Добавление докторов в список доступных пользователей для входа
            InitializeComponent();
            List<doctor> doctors = hospitalEntities.Context.doctor.ToList();
            foreach(doctor d in doctors)
            {
                this.doctors.Add(d.login, d.password);
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Обработка авторизации пользователей
            if (LoginBox.Text == null || LoginBox.Text == "" || PassBox.Text == null || PassBox.Text == "")
                return;
            String login = LoginBox.Text;
            String password = PassBox.Text;
            String right;
            if (doctors.TryGetValue(login, out right))
            {
                if (right == password)
                {
                    Hide();
                    Registration regWin = new Registration();
                    regWin.Show();
                    return;
                }
            }
                    MessageBox.Show("Неверный пароль или логин");
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

    }
}
