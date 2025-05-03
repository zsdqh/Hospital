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
    }
}
