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
    public partial class EditDoctor : Window, IDoctorPage
    {
        // Редактируемый пациент
        public doctor CurrentDoctor { get; set; }
        private bool is_new = false;
        private DoctorsSchedule sched;
        public void LoadRasp(doctor d)
        {
            List<DoctorsScheduleView> days = new List<DoctorsScheduleView>();
            var patientInDb = hospitalEntities.Context.doctor.Find(CurrentDoctor.id);
            if (patientInDb == null)
            {
                CurrentDoctor.id = hospitalEntities.Context.doctor.Select(x => x.id).OrderBy(x => x).ToList().Last() + 1;
                is_new = true;
            }
            DoctorsSchedule sched;
            JSONworker.Context.TryGetValue(CurrentDoctor.id, out sched);
            if (sched == null)
            {
                is_new = true;
                sched = new DoctorsSchedule(new Random());
                JSONworker.Context.Add(CurrentDoctor.id, sched);
                JSONworker.SaveChanges();
            }
            this.sched = sched;
            // Добавление дни недели в таблицу
            days.Add(new DoctorsScheduleView(sched.Monday, "Monday", d.id));
            days.Add(new DoctorsScheduleView(sched.Tuesday, "Tuesday", d.id));
            days.Add(new DoctorsScheduleView(sched.Wednesday, "Wednesday", d.id));
            days.Add(new DoctorsScheduleView(sched.Thursday, "Thursday", d.id));
            days.Add(new DoctorsScheduleView(sched.Friday, "Friday", d.id));
            days.Add(new DoctorsScheduleView(sched.Saturday, "Saturday", d.id));
            ScheduleGrid.ItemsSource = days;
        }

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
            LoadRasp(doctorToEdit);

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
                if (patientInDb == null)
                {
                    patientInDb = CurrentDoctor;
                    hospitalEntities.Context.doctor.Add(patientInDb);
                }
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
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении данных: " + ex.Message);
            }
        }
        private static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T ancestor)
                {
                    return ancestor;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }
        private void ScheduleGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dataGrid = sender as DataGrid;

            // Проверяем, где было совершено нажатие
            var hitTestResult = VisualTreeHelper.HitTest(dataGrid, e.GetPosition(dataGrid));
            if (hitTestResult != null)
            {
                // Ищем родительский элемент типа DataGridRow
                var row = FindAncestor<DataGridRow>(hitTestResult.VisualHit);
                if (row != null && row.IsSelected)
                {
                    // Если строка найдена и выбрана, выполняем действие
                    
                    ScheduleWIndow sw = new ScheduleWIndow(CurrentDoctor.id, dataGrid.SelectedItem as DoctorsScheduleView, this, is_new?CurrentDoctor:null);
                    sw.Show();
                }
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
            if(this.is_new)
            {
                JSONworker.Context.Remove(CurrentDoctor.id);
                JSONworker.SaveChanges();
            }
            this.Close();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Логика удаления врача вместе со связанными визитами и мероприятиями по лечению
            if (MessageBox.Show($"Вы точно хотите удалить сотрудника {CurrentDoctor.name}?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    JSONworker.Context.Remove(CurrentDoctor.id);
                    JSONworker.SaveChanges();
                    // Удаление связанных данных
                    var visitsToRemove = hospitalEntities.Context.visit.Where(v => v.doctor_id == CurrentDoctor.id).ToList();
                    hospitalEntities.Context.visit.RemoveRange(visitsToRemove);
                    var healingEventsToRemove = hospitalEntities.Context.healingevent.Where(he => he.doctor_id == CurrentDoctor.id).ToList();
                    hospitalEntities.Context.healingevent.RemoveRange(healingEventsToRemove);

                    // Удаление доктора
                    hospitalEntities.Context.doctor.Remove(CurrentDoctor);
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
