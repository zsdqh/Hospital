using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Office.Interop.Word;
using Application = Microsoft.Office.Interop.Word.Application;
using System.IO;
using Path = System.IO.Path;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для SpecificPatientPage.xaml
    /// </summary>
    public partial class SpecificPatientPage : System.Windows.Window
    {
        patient _current;
        public SpecificPatientPage(patient p)
        {
            InitializeComponent();
            _current = p;
            DataContext = p;
            SexBlock.Text = p.sex==1 ? "мужской" : "женский";
        }
        public void ChangeTextInDoc(Document doc, string oldText, string newText)
            {
            Range range = doc.Content;
            // Очищаем форматирование перед поиском (если требуется)
            range.Find.ClearFormatting();
            range.Find.Replacement.ClearFormatting();
            // Задаем параметры поиска и замены
            object findText = oldText;     // строка, которую ищем
            object replaceText = newText;    // строка, на которую заменяем
            object missing = Missing.Value;
            // Выполняем поиск и замену во всем диапазоне
            range.Find.Execute(
                ref findText,                // текст для поиска
                ref missing,           
                ref missing,              
                ref missing,              
                ref missing,                
                ref missing,         
                ref missing,                
                ref missing,           
                ref missing,               
                ref replaceText,             // текст для замены
                WdReplace.wdReplaceAll,      // параметр замены («заменить всё»)
                ref missing,            
                ref missing,                
                ref missing,              
                ref missing               
            );
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Договор на мед обслуживание
            //"Шаблон Согласие на обработку ПД.docx";
            Application wordApp = new Application();
            string originalFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Шаблон Согласие на обработку ПД.docx");
            Document doc = wordApp.Documents.Add();
            wordApp.Selection.InsertFile(originalFilePath);

            DateTime now = DateTime.Now;
            ChangeTextInDoc(doc, "[Имя]", _current.name);
            ChangeTextInDoc(doc, "[Фамилия]", _current.second_name);
            ChangeTextInDoc(doc, "[Отчество]", _current.father_name);
            ChangeTextInDoc(doc, "[Паспорт]", _current.passport);
            ChangeTextInDoc(doc, "[Адрес]", _current.adress);
            ChangeTextInDoc(doc, "[День]", String.Format("{0:D2}", now.Day));
            ChangeTextInDoc(doc, "[Месяц]", now.ToString("MMMM"));
            ChangeTextInDoc(doc, "[Год]", now.Year.ToString());
            wordApp.Visible = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Запись на прием
            VisitService vs = new VisitService(_current);
            Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            // Договор на обработку персональных данных
        }
    }
}
