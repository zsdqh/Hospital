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
using System.Windows.Threading;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<string, int> doctors = new Dictionary<string, int>();
        private int custom_hash(string to_hash)
        {
            return ((to_hash.GetHashCode()*11)<<4)/7;
        }
        public void AddVisitsToJson()
        {
            foreach (visit v in hospitalEntities.Context.visit)
            {
                DoctorsSchedule ds;
                JSONworker.Context.TryGetValue(v.doctor_id, out ds);
                List<String> day;
                if (ds.Tickets.TryGetValue(Time.DateToInt(v.date), out day))
                {
                    day.Add(v.date.ToString("HH:mm"));
                }
                else
                {
                    day = new List<String>();
                    day.Add(v.date.ToString("HH:mm"));
                    ds.Tickets.Add(Time.DateToInt((DateTime)v.date), day);
                }
            }
            JSONworker.SaveChanges();
        }
        public void AddEventsToJson()
        {
            foreach (healingevent hv in hospitalEntities.Context.healingevent)
            {
                DoctorsSchedule ds;
                JSONworker.Context.TryGetValue(hv.doctor_id, out ds);
                List<String> day;
                if (ds.Tickets.TryGetValue(Time.DateToInt(hv.date), out day) && day!=null)
                {
                    if (day.Contains("00:00"))
                    {
                        day.RemoveAll(x => x == "00:00");
                    }
                    if (day.Contains(hv.date.ToString("HH:mm")))
                    {
                        continue;
                    }
                    day.Add(hv.date.ToString("HH:mm"));
                }
                else
                {
                    if (day==null)
                    {
                        day = new List<String>();
                        day.Add(hv.date.ToString("HH:mm"));
                        ds.Tickets.Add(Time.DateToInt((DateTime)hv.date), day);
                    }
                    if (day.Contains("00:00"))
                    {
                        day.RemoveAll(x=>x=="00:00");
                    }

                }
            }
            JSONworker.SaveChanges();
        }
        public MainWindow()
        {
            // Добавление докторов в список доступных пользователей для входа
            InitializeComponent();
            List<doctor> doctors = hospitalEntities.Context.doctor.ToList();
            foreach(doctor d in doctors)
            {
                this.doctors.Add(d.login, custom_hash(d.password));
            }
            this.PassBox.Password = "Пароль";
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Обработка авторизации пользователей
            if (LoginBox.Text == null || LoginBox.Text == "" || PassBox.Password == null || PassBox.Password == "")
                return;
            String login = LoginBox.Text;
            String password = PassBox.Password;
            int right;
            if (doctors.TryGetValue(login, out right))
            {
                if (right == custom_hash(password))
                {
                    Hide();
                    Registration regWin = new Registration();
                    regWin.Show();
                    return;
                }
            }

            MessageBox.Show("Неверный пароль или логин");
            LoInButton.IsEnabled = false;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += (s, ee) =>
            {
                LoInButton.IsEnabled = true;
                timer.Stop();
            };
            timer.Start();

        }

        private void LoginBox_GotFocus(object sender, RoutedEventArgs e)
        {
            LoginBox.Text = "";
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            LoginBox.Foreground = brush;
        }

        private void PassBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PassBox.Password = "";
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            PassBox.Foreground = brush;

        }

    }
}
