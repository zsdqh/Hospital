﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
            Man.IsChecked = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = true;

            void Validate(TextBox box, string pattern = null)
            {
                if (string.IsNullOrWhiteSpace(box.Text) || (pattern != null && !System.Text.RegularExpressions.Regex.IsMatch(box.Text, pattern)))
                {
                    box.Foreground = Brushes.Red;
                    isValid = false;
                }
                else
                {
                    box.Foreground = Brushes.Black;
                }
            }
            // Валидация данных пациента
            Validate(NameBox);
            Validate(SecondNameBox);
            Validate(FatherNameBox);
            Validate(PassportBox, @"BY\d{4}[A-Z]\d{3}[A-Z]{2}\d");
            Validate(PhoneBox, @"\+375\((25|44|33|29)\)\d{3}-\d{2}-\d{2}");
            Validate(EmailBox, @"^[a-zA-Z0-9._%+-]+@(gmail\.com|mail\.ru)$");
            Validate(AdressBox);
            Validate(LoginBox);
            Validate(PasswordBox, @".{4,}");
            birth.Foreground = Brushes.Black;
            DateTime? birthday = birth.SelectedDate;
            if (birthday == null || birthday>=DateTime.Now)
            {
                birth.Foreground = Brushes.Red;
                isValid = false;
            }

            if (isValid)
            {
                patient d = new patient();
                d.id=hospitalEntities.Context.patient.ToList().Max(x => x.id) + 1;
                d.name = NameBox.Text;
                d.second_name = SecondNameBox.Text;
                d.father_name = FatherNameBox.Text;
                d.passport = PassportBox.Text;
                // Обработка уникальности данных
                if (hospitalEntities.Context.patient.Select(x=>x.passport).ToHashSet().Contains(d.passport))
                        {
                    Validate(PassportBox, "ERRORVALUE");
                    goto error;
                }
                d.birthday = birth.SelectedDate.Value;
                d.sex = (bool)Man.IsChecked ? 1 : 0;
                d.adress = AdressBox.Text;
                d.phone = PhoneBox.Text;
                if (hospitalEntities.Context.patient.Select(x => x.phone).ToHashSet().Contains(d.phone))
                {
                    Validate(PhoneBox, "ERRORVALUE");
                    goto error;
                }
                d.email = EmailBox.Text;
                if (hospitalEntities.Context.patient.Select(x => x.email).ToHashSet().Contains(d.email))
                {
                    Validate(EmailBox, "ERRORVALUE");
                    goto error;
                }
                d.card_number = hospitalEntities.Context.patient.ToList().Max(x => x.card_number) + 1;
                d.card_date = DateTime.Now;
                d.last_visit = DateTime.Now;
                d.next_visit = DateTime.Now;
                d.card_date = d.card_date.AddYears(4);
                d.policy_number = hospitalEntities.Context.patient.ToList().Max(x => x.policy_number) + 1;
                d.policy_end = d.card_date;
                d.login = LoginBox.Text;
                d.password = PasswordBox.Text;
                if(File.Exists(PhotoPathBox.Text))
                {
                    d.photo = PhotoPathBox.Text;
                }
                try
                {
                    hospitalEntities.Context.patient.Add(d);
                    hospitalEntities.Context.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                        {
                            Console.WriteLine($"Свойство: {validationError.PropertyName} Ошибка: {validationError.ErrorMessage}");
                        }
                    }
                }

            }
        error:
            MessageBox.Show(isValid ? "Все данные введены корректно!" : "Некоторые поля заполнены неверно. Проверьте и исправьте их.");
        }

        private void SelectPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            // Обработка добавления фотографии
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                PhotoPathBox.Text = openFileDialog.FileName;
            }
        }

        private void WorkBox_GotFocus(object sender, RoutedEventArgs e)
        {
            LoginBox.Text = "";
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            LoginBox.Foreground = brush;
        }

        private void StrahBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox.Text = "";
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            PasswordBox.Foreground = brush;

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Hide();
            MainWindow main = new MainWindow();
            main.Show();
        }

        private void NameBox_GotFocus(object sender, RoutedEventArgs e)
        {
            NameBox.Text = "";
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            NameBox.Foreground = brush;
        }

        private void SecondNameBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SecondNameBox.Text = "";
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            SecondNameBox.Foreground = brush;
        }

        private void FatherNameBox_GotFocus(object sender, RoutedEventArgs e)
        {
            FatherNameBox.Text = "";
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            FatherNameBox.Foreground = brush;
        }

        private void PassportBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PassportBox.Text = "";
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            PassportBox.Foreground = brush;
        }

        private void AdressBox_GotFocus(object sender, RoutedEventArgs e)
        {
            AdressBox.Text = "";
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            AdressBox.Foreground = brush;
        }

        private void PhoneBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PhoneBox.Text = "";
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            PhoneBox.Foreground = brush;
        }

        private void EmailBox_GotFocus(object sender, RoutedEventArgs e)
        {
            EmailBox.Text = "";
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            EmailBox.Foreground = brush;
        }
        // Обработка выбора пола
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
        //Перенос в другие окна
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            DBService dbWindow = new DBService();
            dbWindow.Show();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            PatientPage patient = new PatientPage();
            patient.Show();
        }
    }
}
