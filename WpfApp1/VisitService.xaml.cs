using System;
using System.Collections.Generic;
using System.Drawing;
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
using Brushes = System.Windows.Media.Brushes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для VisitService.xaml
    /// </summary>
    public partial class VisitService : Window
    {
        patient _current_patient;
        private bool flag; // true - запись на прием    |    false - запись на мероприятие

        public VisitService(patient p)
        {
            InitializeComponent();
            _current_patient = p;
            flag = true;
            FlagChanged();
        }
        private void FlagChanged()
        {
            SolidColorBrush minty = new SolidColorBrush(System.Windows.Media.Color.FromRgb(200, 250, 225));
            if (flag)
            {
                Priem.Background = minty;
                Meropr.Background = Brushes.Gray;
                MainFrame.Navigate(new Priem());
                Priem.BorderThickness = new Thickness(3);
                Meropr.BorderThickness = new Thickness(0);
            }
            else
            {
                Meropr.Background = minty;
                Priem.Background = Brushes.Gray;
                MainFrame.Navigate(new Meropr());
                Priem.BorderThickness = new Thickness(0);
                Meropr.BorderThickness = new Thickness(3);

            }
        }

        private void Priem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!flag)
            {
                flag = true;
                FlagChanged();
            }
        }

        private void Meropr_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(flag)
            {
                flag = false;
                FlagChanged();
            }
        }
    }
}
