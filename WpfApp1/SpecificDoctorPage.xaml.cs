using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;
using Excel = Microsoft.Office.Interop.Excel;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для SpecificDoctorPage.xaml
    /// </summary>
    public class VisitView
    {
        public Time time_ { get; set; }
        public string time { get { return time_.ToString(); } }
        public string fio {  get; set; }
        public visit parent {  get; set; }
        public VisitView(visit v)
        {
            parent = v;
            patient p = v.patient;
            fio = p.second_name + " " + p.name + " " + p.father_name;
            time_ = new Time(v.date.ToString("HH:mm"));
        }
    }
    public partial class SpecificDoctorPage : Window, IDoctorPage
    {

        public doctor _currentDoctor = new doctor();
        public void LoadRasp(doctor d)
        {
            List<DoctorsScheduleView> days = new List<DoctorsScheduleView>();
            DoctorsSchedule sched;
            JSONworker.Context.TryGetValue(_currentDoctor.id, out sched);
            // Добавление дни недели в таблицу
            days.Add(new DoctorsScheduleView(sched.Monday, "Monday", d.id));
            days.Add(new DoctorsScheduleView(sched.Tuesday, "Tuesday", d.id));
            days.Add(new DoctorsScheduleView(sched.Wednesday, "Wednesday", d.id));
            days.Add(new DoctorsScheduleView(sched.Thursday, "Thursday", d.id));
            days.Add(new DoctorsScheduleView(sched.Friday, "Friday", d.id));
            days.Add(new DoctorsScheduleView(sched.Saturday, "Saturday", d.id));
            ScheduleGrid.ItemsSource = days;
        }
        public SpecificDoctorPage(doctor d)
        {
            InitializeComponent();
            _currentDoctor = d;
            DataContext = _currentDoctor;
            SexBlock.Text = _currentDoctor.sex==1 ? "Мужской" : "Женский";
            LoadRasp(d);
            picker.SelectedDate = DateTime.Now;

        }
        // Вспомогательный метод для поиска предка указанного типа
        private static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T ancestor)
                {
                    return ancestor;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }

        private void ScheduleGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dataGrid = sender as DataGrid;

            // Проверяем, где было совершено нажатие
            var hitTestResult = VisualTreeHelper.HitTest(dataGrid, e.GetPosition(dataGrid));
            if (hitTestResult != null)
            {
                // Ищем родительский элемент типа DataGridRow
                var row = FindAncestor<DataGridRow>(hitTestResult.VisualHit);
                if (row != null && row.IsSelected)
                {
                    // Если строка найдена и выбрана, выполняем действие
                    ScheduleWIndow sw = new ScheduleWIndow(_currentDoctor.id, dataGrid.SelectedItem as DoctorsScheduleView, this);
                    sw.Show();
                }
            }
        }
        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime chosen = (DateTime)picker.SelectedDate;
            List<VisitView> views = new List<VisitView>();
            foreach (visit v in hospitalEntities.Context.visit.Where(x => x.doctor_id == _currentDoctor.id && x.date.Year==chosen.Year && x.date.Month==chosen.Month && x.date.Day==chosen.Day))
            {
                views.Add(new VisitView(v));
            }
            talonsBox.ItemsSource = views;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (talonsBox.SelectedItem == null || picker.SelectedDate==null)
                return;
            DoctorsSchedule ds;
            JSONworker.Context.TryGetValue(_currentDoctor.id, out ds);
            List<String> day;
            ds.Tickets.TryGetValue(Time.DateToInt((DateTime)picker.SelectedDate), out day);
            day.Remove((talonsBox.SelectedItem as VisitView).time);
            if (day.Count==0)
            {
                ds.Tickets.Remove(Time.DateToInt((DateTime)picker.SelectedDate));
            }
            hospitalEntities.Context.visit.Remove((talonsBox.SelectedItem as VisitView).parent);
            hospitalEntities.Context.SaveChanges();
            JSONworker.SaveChanges();
            DatePicker_SelectedDateChanged(sender, e as SelectionChangedEventArgs);
        }
    }
}
