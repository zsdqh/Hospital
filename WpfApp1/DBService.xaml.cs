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
            CrudService cs= new CrudService(_currentDoctor, this);
            cs.Show();
            this.Visibility = Visibility.Hidden;

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            CrudService cs = new CrudService(new doctor(), this);
            cs.Show();
            this.Visibility = Visibility.Hidden;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show($"Вы точно хотите удалить сотрудника {_currentDoctor.name}?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question)==MessageBoxResult.Yes)
            {
                try
                {
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
            if(Visibility == Visibility.Visible)
            {

            hospitalEntities.Context.ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
            HotelDataGrid.ItemsSource = hospitalEntities.Context.doctor.ToList();
            }
        }
    }
}
