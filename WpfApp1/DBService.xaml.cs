using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Security.Cryptography;
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
    /// Логика взаимодействия для DBService.xaml
    /// </summary>
    public partial class DBService : Window
    {
        private doctor _currentDoctor = new doctor();
        public DBService()
        {
            InitializeComponent();
            HotelDataGrid.ItemsSource = hospitalEntities.Context.doctor.ToList();
        }

        private void HotelDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentDoctor = (doctor)HotelDataGrid.CurrentItem;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Отображение окна редактирования
            CrudService cs= new CrudService(_currentDoctor, this);
            cs.Show();
            this.Visibility = Visibility.Hidden;

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Отображение окна создания
            CrudService cs = new CrudService(new doctor(), this);
            cs.Show();
            this.Visibility = Visibility.Hidden;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            // Логика удаления врача вместе со связанными визитами и мероприятиями по лечению
            if(MessageBox.Show($"Вы точно хотите удалить сотрудника {_currentDoctor.name}?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question)==MessageBoxResult.Yes)
            {
                try
                {
                    // Удаление связанных данных
                    var visitsToRemove = hospitalEntities.Context.visit.Where(v => v.doctor_id == _currentDoctor.id).ToList();
                    hospitalEntities.Context.visit.RemoveRange(visitsToRemove);
                    var healingEventsToRemove = hospitalEntities.Context.healingevent.Where(he => he.doctor_id == _currentDoctor.id).ToList();
                    hospitalEntities.Context.healingevent.RemoveRange(healingEventsToRemove);

                    // Удаление доктора
                    hospitalEntities.Context.doctor.Remove(_currentDoctor);
                    hospitalEntities.Context.SaveChanges();
                    MessageBox.Show("Удалено успешно");
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            this.Visibility = Visibility.Hidden;
            this.Visibility = Visibility.Visible;
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Обновление таблицы врачей
            if(Visibility == Visibility.Visible)
            {
            hospitalEntities.Context.ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
            HotelDataGrid.ItemsSource = hospitalEntities.Context.doctor.ToList();
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            // Обработка вывода отчета о проведенных приемах врачей
            var allDoctors = hospitalEntities.Context.doctor.OrderBy(x => x.second_name).ToList();

            var application = new Excel.Application();
            application.SheetsInNewWorkbook = allDoctors.Count();
            Excel.Workbook workbook = application.Workbooks.Add(Type.Missing);

            for(int i = 0;i<allDoctors.Count;i++)
            {
                int startRowIndex = 1;
                Excel.Worksheet worksheet = application.Worksheets.Item[i + 1];
                doctor d = allDoctors[i];
                worksheet.Name = d.second_name + " " + d.name;
                // Заголовки таблицы
                worksheet.Cells[1][startRowIndex] = "Дата проведения";
                worksheet.Cells[2][startRowIndex] = "Пациент";
                worksheet.Cells[3][startRowIndex] = "Результат";
                startRowIndex++;
                // Добавление визитов
                foreach(visit vis in hospitalEntities.Context.visit.Where(x=>x.doctor_id==d.id))
                {
                    patient p = hospitalEntities.Context.patient.Where(x => x.id == vis.patient_id).First();
                    worksheet.Cells[1][startRowIndex] = vis.date.ToString("dd.MM.yyyy HH:mm");
                    worksheet.Cells[2][startRowIndex] = p.second_name + " " + p.name + " " + p.father_name;
                    worksheet.Cells[3][startRowIndex] = vis.result;

                    startRowIndex++;
                }
                // Добавление лечебных мероприятий
                foreach(healingevent vis in hospitalEntities.Context.healingevent.Where(x=>x.doctor_id==d.id))
                {
                    patient p = hospitalEntities.Context.patient.Where(x => x.id == vis.patient_id).First();
                    worksheet.Cells[1][startRowIndex] = vis.date.ToString("dd.MM.yyyy HH:mm");
                    worksheet.Cells[2][startRowIndex] = p.second_name + " " + p.name + " " + p.father_name;
                    worksheet.Cells[3][startRowIndex] = vis.result;
                    startRowIndex++;
                }
                Excel.Range totalRange = worksheet.Range[worksheet.Cells[1][startRowIndex], worksheet.Cells[2][startRowIndex]];
                totalRange.Merge();
                totalRange.Value = "Всего приемов:";
                totalRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                worksheet.Cells[3][startRowIndex].Formula2 = $"=COUNTA(A2:A{startRowIndex - 1})";

                // Создание границ
                Excel.Range rangeBorders = worksheet.Range[worksheet.Cells[1][1], worksheet.Cells[3][startRowIndex]];
                rangeBorders.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle =
                rangeBorders.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle =
                rangeBorders.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle =
                rangeBorders.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle =
                rangeBorders.Borders[Excel.XlBordersIndex.xlInsideHorizontal].LineStyle =
                rangeBorders.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                worksheet.Columns.AutoFit();
            }
            application.Visible = true;
        }
    }
}
