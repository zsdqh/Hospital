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

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для PatientPage.xaml
    /// </summary>
    public partial class PatientPage : Window
    {
        public PatientPage()
        {
            InitializeComponent();
            var currentPatients = hospitalEntities.Context.patient.ToList();
            Patients.ItemsSource = currentPatients;
            foreach (var currentPatient in currentPatients)
                currentPatient.photo = "pack://siteoforigin:,,,/" + currentPatient.photo;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var currentPatients = hospitalEntities.Context.patient.ToList();

            if(TBoxSearch.Text != null && TBoxSearch.Text != "")
                currentPatients = currentPatients.Where(p => p.second_name.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();
            
            if (TBoxSearchSecond.Text != null && TBoxSearchSecond.Text != "")
                currentPatients = currentPatients.Where(p => p.card_number.ToString().Contains(TBoxSearchSecond.Text)).ToList();

            Patients.ItemsSource = currentPatients;
        }
    }
}
