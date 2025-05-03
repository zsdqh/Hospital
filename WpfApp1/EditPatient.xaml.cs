using Microsoft.Win32;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApp1
{
    public partial class EditPatient : Window
    {
        // Редактируемый пациент
        public patient CurrentPatient { get; set; }

        // Конструктор принимает объект пациента для редактирования
        public EditPatient(patient patientToEdit)
        {
            InitializeComponent();
            CurrentPatient = patientToEdit;
            // Устанавливаем привязку данных для автоматического отображения значений в элементах управления
            this.DataContext = CurrentPatient;
            if (patientToEdit.sex==1)
            {
                Man.IsChecked = true;
            }
            else
            {
                Woman.IsChecked = true;
            }
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
                CurrentPatient.photo = openFileDialog.FileName;
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
            Validate(EmailBox, @"^[a-zA-Z0-9._%+-]+@(gmail\.com|mail\.ru)$");
            Validate(AdressBox);
            Validate(LoginBox);
            Validate(PasswordBox, @".{4,}");

            // Валидация даты рождения
            birth.Foreground = Brushes.Black;
            DateTime? birthday = birth.SelectedDate;
            if (birthday == null || birthday >= DateTime.Now || birthday.Value.Year<1900)
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
                if (hospitalEntities.Context.patient
                        .Where(x => x.id != CurrentPatient.id)
                        .Select(x => x.passport)
                        .Contains(PassportBox.Text))
                {
                    Validate(PassportBox, "ERRORVALUE");
                    MessageBox.Show("Паспорт неуникален.");
                    return;
                }

                if (hospitalEntities.Context.patient
                        .Where(x => x.id != CurrentPatient.id)
                        .Select(x => x.phone)
                        .Contains(PhoneBox.Text))
                {
                    Validate(PhoneBox, "ERRORVALUE");
                    MessageBox.Show("Телефон неуникален.");
                    return;
                }

                if (hospitalEntities.Context.patient
                        .Where(x => x.id != CurrentPatient.id)
                        .Select(x => x.email)
                        .Contains(EmailBox.Text))
                {
                    Validate(EmailBox, "ERRORVALUE");
                    MessageBox.Show("Почта неуникальна.");
                    return;
                }

                // Находим пациента в базе данных по его ID
                var patientInDb = hospitalEntities.Context.patient.Find(CurrentPatient.id);
                if (patientInDb != null)
                {
                    // Обновляем свойства. Здесь можно использовать данные из контролов, т.к. они содержат актуальные отредактированные значения:
                    patientInDb.name = NameBox.Text;
                    patientInDb.second_name = SecondNameBox.Text;
                    patientInDb.father_name = FatherNameBox.Text;
                    patientInDb.passport = PassportBox.Text;
                    patientInDb.birthday = birth.SelectedDate.Value;
                    patientInDb.sex = (bool)Man.IsChecked?1:0;  
                    patientInDb.adress = AdressBox.Text;
                    patientInDb.phone = PhoneBox.Text;
                    patientInDb.email = EmailBox.Text;
                    patientInDb.diagnosis = DiagnosisBox.Text;
                    patientInDb.history = HistoryBox.Text;
                    patientInDb.photo = CurrentPatient.photo;
                    patientInDb.login = LoginBox.Text;
                    patientInDb.password = PasswordBox.Text;

                    hospitalEntities.Context.SaveChanges();
                    MessageBox.Show("Данные сохранены успешно!");
                    this.DialogResult = true;
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
            this.DialogResult = false;
            this.Close();
        }

        private void Image_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Логика удаления пациента вместе со связанными визитами и мероприятиями по лечению
            if (MessageBox.Show($"Вы точно хотите УДАЛИТЬ пациента {CurrentPatient.name}?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    // Удаление связанных данных
                    var visitsToRemove = hospitalEntities.Context.visit.Where(v => v.patient_id == CurrentPatient.id).ToList();
                    hospitalEntities.Context.visit.RemoveRange(visitsToRemove);
                    var healingEventsToRemove = hospitalEntities.Context.healingevent.Where(he => he.patient_id == CurrentPatient.id).ToList();
                    hospitalEntities.Context.healingevent.RemoveRange(healingEventsToRemove);

                    // Удаление пациента
                    hospitalEntities.Context.patient.Remove(CurrentPatient);
                    hospitalEntities.Context.SaveChanges();
                    MessageBox.Show("Удалено успешно");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            Close();
        }
    }
}
