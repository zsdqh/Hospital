using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Логика взаимодействия для CrudService.xaml
    /// </summary>
    public partial class CrudService : Window
    {

        public doctor _currentDoctor = new doctor();
        public DBService parent;
        public CrudService(doctor d, DBService parent)
        {
            InitializeComponent();
            this.parent = parent;
            _currentDoctor = d;
            DataContext = _currentDoctor;
            List<String> specs = hospitalEntities.Context.doctor.ToList().Select(x => x.specialization).ToList().ToHashSet().ToList();
            Debug.WriteLine(specs.Count);
            ComboDoctors.ItemsSource = specs;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentDoctor.name))
                errors.AppendLine("Укажите имя врача");
            if (string.IsNullOrWhiteSpace(_currentDoctor.second_name))
                errors.AppendLine("Укажите фамилию врача");
            if (string.IsNullOrWhiteSpace(_currentDoctor.father_name))
                errors.AppendLine("Укажиет отчество врача");
            if (string.IsNullOrWhiteSpace(_currentDoctor.specialization))
                errors.AppendLine("Укажите специализацию");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
            if (_currentDoctor.id==0)
            {
                _currentDoctor.id=hospitalEntities.Context.doctor.ToList().Max(x => x.id)+1;
                hospitalEntities.Context.doctor.Add(_currentDoctor);
            }
            try
            {
                hospitalEntities.Context.SaveChanges();
                MessageBox.Show("Сохранено");
            }
            catch(Exception  ex)
            {
                MessageBox.Show(ex.ToString());
            }
            parent.Visibility = Visibility.Visible;
            
            this.Close();
        }
    }
}
