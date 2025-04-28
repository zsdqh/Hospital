using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.History;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для EditDoctor.xaml
    /// </summary>
    public partial class EditDoctor : Window
    {
        // Редактируемый пациент
        public doctor CurrentDoctor { get; set; }

        // Конструктор принимает объект пациента для редактирования
        public EditDoctor(doctor doctorToEdit)
        {
            InitializeComponent();
            CurrentDoctor = doctorToEdit;
            // Устанавливаем привязку данных для автоматического отображения значений в элементах управления
            this.DataContext = CurrentDoctor;
            if (doctorToEdit.sex == 1)
            {
                Man.IsChecked = true;
            }
            else
            {
                Woman.IsChecked = true;
            }
            this.SpecBox.ItemsSource = hospitalEntities.Context.doctor.Select(x => x.specialization).ToHashSet().ToList();
        }

        // Обработчик для выбора фотографии
        private void SelectPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Обновляем путь к фото в объекте пациента и, если есть, в TextBox
                CurrentDoctor.photo = openFileDialog.FileName;
                PhotoPathBox.Text = openFileDialog.FileName;
            }
        }

        // Обработчик кнопки "Сохранить"
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = true;

            // Локальная функция валидации для TextBox.
            void Validate(TextBox box, string pattern = null)
            {
                if (string.IsNullOrWhiteSpace(box.Text) ||
                   (pattern != null && !Regex.IsMatch(box.Text, pattern)))
                {
                    box.Foreground = Brushes.Red;
                    isValid = false;
                }
                else
                {
                    box.Foreground = Brushes.Black;
                }
            }

            // Валидация полей редактирования
            Validate(NameBox);
            Validate(SecondNameBox);
            Validate(FatherNameBox);
            Validate(PassportBox, @"BY\d{4}[A-Z]\d{3}[A-Z]{2}\d");
            Validate(PhoneBox, @"\+375\((25|44|33|29)\)\d{3}-\d{2}-\d{2}");
            Validate(AdressBox);
            Validate(LoginBox);
            Validate(PasswordBox, @".{4,}");

            // Валидация даты рождения
            birth.Foreground = Brushes.Black;
            DateTime? birthday = birth.SelectedDate;
            if (birthday == null || birthday >= DateTime.Now || birthday.Value.Year < 1900)
            {
                isValid = false;
            }

            if (!isValid)
            {
                MessageBox.Show("Некоторые поля заполнены неверно. Проверьте и исправьте их.");
                return;
            }

            try
            {
                // Проверка уникальности данных для редактируемого пациента (исключая самого себя)
                if (hospitalEntities.Context.doctor
                        .Where(x => x.id != CurrentDoctor.id)
                        .Select(x => x.passport)
                        .Contains(PassportBox.Text))
                {
                    Validate(PassportBox, "ERRORVALUE");
                    MessageBox.Show("Паспорт неуникален.");
                    return;
                }

                if (hospitalEntities.Context.doctor
                        .Where(x => x.id != CurrentDoctor.id)
                        .Select(x => x.phone)
                        .Contains(PhoneBox.Text))
                {
                    Validate(PhoneBox, "ERRORVALUE");
                    MessageBox.Show("Телефон неуникален.");
                    return;
                }

               

                // Находим пациента в базе данных по его ID
                var patientInDb = hospitalEntities.Context.doctor.Find(CurrentDoctor.id);
                if (patientInDb != null)
                {
                    // Обновляем свойства. Здесь можно использовать данные из контролов, т.к. они содержат актуальные отредактированные значения:
                    patientInDb.name = NameBox.Text;
                    patientInDb.second_name = SecondNameBox.Text;
                    patientInDb.father_name = FatherNameBox.Text;
                    patientInDb.passport = PassportBox.Text;
                    patientInDb.birthday = birth.SelectedDate.Value;
                    patientInDb.sex = (bool)Man.IsChecked ? 1 : 0;
                    patientInDb.adress = AdressBox.Text;
                    patientInDb.phone = PhoneBox.Text;
                    patientInDb.post = PostBox.Text;
                    patientInDb.specialization = SpecBox.Text;
                    patientInDb.photo = CurrentDoctor.photo;
                    patientInDb.login = LoginBox.Text;
                    patientInDb.password = PasswordBox.Text;

                    hospitalEntities.Context.SaveChanges();
                    MessageBox.Show("Данные сохранены успешно!");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Пациент не найден в базе данных.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении данных: " + ex.Message);
            }
        }
        private void Man_Click(object sender, RoutedEventArgs e)
        {
            if (Woman.IsChecked == true)
            {
                Woman.IsChecked = false;
            }
        }

        private void Woman_Click(object sender, RoutedEventArgs e)
        {
            if (Man.IsChecked == true)
            {
                Man.IsChecked = false;
            }
        }

        // Обработчик кнопки "Отмена"
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
