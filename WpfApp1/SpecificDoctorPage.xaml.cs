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
    public partial class SpecificDoctorPage : Window
    {

        public doctor _currentDoctor = new doctor();
        public SpecificDoctorPage(doctor d)
        {
            InitializeComponent();
            _currentDoctor = d;
            DataContext = _currentDoctor;
            SexBlock.Text = _currentDoctor.sex==1 ? "Мужской" : "Женский";
        }

    }
}
