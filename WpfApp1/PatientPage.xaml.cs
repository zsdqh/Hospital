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
            /*
                Добавление к пути каждой фотографии префикса о том, что ссылка является абсолютной 
             */
            foreach (var currentPatient in currentPatients)
            {
                if(! currentPatient.photo.StartsWith("pack://siteoforigin:,,,/"))
                {
                    currentPatient.photo = "pack://siteoforigin:,,,/" + currentPatient.photo;
                }
            }
            hospitalEntities.Context.SaveChanges();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var currentPatients = hospitalEntities.Context.patient.ToList();
            // Обработка фильтрации пациентов

            if(TBoxSearch.Text != null && TBoxSearch.Text != "")
                currentPatients = currentPatients.Where(p => p.second_name.ToLower().StartsWith(TBoxSearch.Text.ToLower())).ToList();
            
            if (TBoxSearchSecond.Text != null && TBoxSearchSecond.Text != "")
                currentPatients = currentPatients.Where(p => p.card_number.ToString().StartsWith(TBoxSearchSecond.Text)).ToList();

            Patients.ItemsSource = currentPatients;
        }

        // Открытие окон просмотра и редактирования
        private void Item_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            SpecificPatientPage spp = new SpecificPatientPage((sender as ListViewItem).DataContext as patient);
            spp.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
