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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для Priem.xaml
    /// </summary>
    public partial class Priem : Page
    {
        public Priem()
        {
            InitializeComponent();
            SpecCombo.ItemsSource = hospitalEntities.Context.doctor.Select(x => x.specialization).ToHashSet().ToList();
            SpecCombo.Text = "терапевт";

        }

        private void SpecCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            doctorsAvailable.ItemsSource = hospitalEntities.Context.doctor.Where(x => x.specialization == (string)SpecCombo.SelectedItem).Select(x=>x.specialization).ToList();
        }
    }
}
