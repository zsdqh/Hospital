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

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для ScheduleWIndow.xaml
    /// </summary>
    public partial class ScheduleWIndow : Window
    {
        private int doctor_id;
        private DoctorsSchedule day;
        private DoctorsScheduleView dayView;
        private IDoctorPage parent;
        public ScheduleWIndow(int doctor_id, DoctorsScheduleView dayView, IDoctorPage parent)
        {
            InitializeComponent();
            this.dayView = dayView;
            this.parent = parent;
            JSONworker.Context.TryGetValue(dayView.Id, out day);

            bool isActive = day[dayView.EngName]!=null && day[dayView.EngName][2] != null;
            ActivatePanel(SP1, isActive);
            ActivatePanel(SP2, isActive);
            this.IsChet.IsChecked = isActive;
            this.doctor_id = doctor_id;


            if (dayView.Text!="-")
            {
                Time startChetTime = new Time(day[dayView.EngName][0]);
                Time endChetTime = new Time(day[dayView.EngName][1]);
                startChet.Hours = startChetTime.Hours;
                startChet.Minutes = startChetTime.Minutes;
                endChet.Hours = endChetTime.Hours;
                endChet.Minutes = endChetTime.Minutes;
                if(day[dayView.EngName][2]!=null)
                {
                    Time startNechetTime = new Time(day[dayView.EngName][2]);
                    Time endNechetTime = new Time(day[dayView.EngName][3]);
                    startNechet.Hours = startNechetTime.Hours;
                    startNechet.Minutes = startNechetTime.Minutes;
                    endNechet.Hours = endNechetTime.Hours;
                    endNechet.Minutes = endNechetTime.Minutes;
                }
            }
        }
        public void ActivatePanel(StackPanel p, bool state) // Метод активации\деактивации элементов управления внутри StackPanel
        {
            // Прохождение по всем элементам StackPanel
            foreach (UIElement o in p.Children)
            {
                o.IsEnabled = state;
                // Прохождение по всем элементам вложенного DockPanel
                if (o is DockPanel)
                {
                    foreach(UIElement ob in (o as DockPanel).Children)
                    {
                        ob.IsEnabled = state;
                    }
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if((bool)IsChet.IsChecked)
            {
                ActivatePanel(SP1, true);
                ActivatePanel(SP2, true);
            }
            else
            {
                ActivatePanel(SP1, false);
                ActivatePanel(SP2, false);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<String> toSave = new List<string> { null, null, null, null };
            Time startChetTime = new Time(startChet.Hours, startChet.Minutes);
            Time endChetTime = new Time(endChet.Hours, endChet.Minutes);
            Time startNechetTime = new Time(startNechet.Hours, startNechet.Minutes);
            Time endNechetTime = new Time(endNechet.Hours, endNechet.Minutes);
            if(startChetTime.CompareTo(endChetTime)>0 || startNechetTime.CompareTo(endNechetTime)>0)
            {
                MessageBox.Show("Начальное время не может превышать завершающее.");
                return;
            }
            if (startChetTime.CompareTo(endChetTime)==0)
            {
                toSave = null;
                goto save;
            }
            toSave[0] = startChetTime.ToString();
            toSave[1] = endChetTime.ToString();
            if (startNechetTime.CompareTo(endNechetTime)==0)
                goto save;
            toSave[2]=startNechetTime.ToString();
            toSave[3]=endNechetTime.ToString();

        save:
            day[dayView.EngName] =  toSave;
            JSONworker.SaveChanges();
            parent.LoadRasp(hospitalEntities.Context.doctor.Where(x => x.id == doctor_id).First());
            Close();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            day[dayView.EngName] = null;
            JSONworker.SaveChanges();
            parent.LoadRasp(hospitalEntities.Context.doctor.Where(x => x.id == doctor_id).First());
            Close();
        }
    }
}
