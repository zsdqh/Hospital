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
    /// Логика взаимодействия для CrudService.xaml
    /// </summary>
    public partial class CrudService : Window
    {
        public doctor _currentDoctor = new doctor();
        public CrudService()
        {
            InitializeComponent();
            DataContext = _currentDoctor;
            //ComboDoctors.ItemSource = hospitalEntities.Context.doctor.ToList();
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
        }
    }
}
