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
using System.Collections;
using Word = Microsoft.Office.Interop.Word;
namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для SpecificPatientPage.xaml
    /// </summary>
    public class VisitItem // Класс для отображения визитов и мероприятий в ListBox
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string Date {  get; set; }
        public bool IsVisit {  get; set; } // true - визит    |   false - мероприятие
        public int doctor_id {  get; set; }
        public int patient_id {  get; set; }
        public VisitItem(int id, bool isVisit, DateTime date,  string name, int doctor_id, int patient_id)
        {
            this.Id = id;
            this.IsVisit = isVisit;
            this.Date = date.ToString("dd.MM.yyyy HH:mm");
            this.Name = name;
            this.doctor_id = doctor_id;
            this.patient_id = patient_id;
        }
    }

    public partial class SpecificPatientPage : System.Windows.Window
    {
        private string ToUpperFirst(string s)
        {
            return Char.ToUpper(s[0]) + s.Remove(0, 1);
        }
        patient _current;
        public SpecificPatientPage(patient p)
        {
            InitializeComponent();
            _current = p;
            DataContext = p;
            SexBlock.Text = p.sex == 1 ? "мужской" : "женский";

            List<VisitItem> visits = new List<VisitItem>();
            foreach(visit v in hospitalEntities.Context.visit.Where(x=>x.patient_id==_current.id))
            {
                visits.Add(new VisitItem(v.id, true, v.date, "Визит: " + hospitalEntities.Context.doctor.Where(x => x.id == v.doctor_id).First().specialization, v.doctor_id, v.patient_id));
            }
            foreach(healingevent hv in hospitalEntities.Context.healingevent.Where(x=>x.patient_id==_current.id))
            {
                string type = hv.type;
                type = ToUpperFirst(type);
                if (hv.date.Hour == 0 && hv.date.Minute == 0) // Если время не было задано, то задать случайное
                {
                    hv.date = hv.date.AddHours((new Random()).Next(0, 24));
                    hv.date = hv.date.AddMinutes((new Random()).Next(0, 12) * 5);
                }
                visits.Add(new VisitItem(hv.id, false, hv.date,  type + ": " + hv.name, hv.doctor_id, hv.patient_id));
            }
            try
            {
                    hospitalEntities.Context.SaveChanges();

            }catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        MessageBox.Show($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                    }
                }
            }
            allVisits.ItemsSource = visits;
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
            //Договор на обработку персональных данных
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
            vs.Show();
            Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            // Формирование отчета по выбранному мероприятию
            if (allVisits.SelectedItem == null) return;
            VisitItem selected = allVisits.SelectedItem as VisitItem;
            patient current_patient = hospitalEntities.Context.patient.Where(x => x.id == selected.patient_id).First();
            doctor current_doctor = hospitalEntities.Context.doctor.Where(x => x.id == selected.doctor_id).First();
            var application = new Word.Application();

            Word.Document document = application.Documents.Add();
            Word.Paragraph userParagraph = document.Paragraphs.Add();
            Word.Range userRange = userParagraph.Range;
            userRange.Text = selected.Name;
            userRange.set_Style("Заголовок 1");
            userRange.InsertParagraphAfter();

            Word.Paragraph tableParagraph = document.Paragraphs.Add();
            Word.Range tableRange = tableParagraph.Range;
            Word.Table visitTable = document.Tables.Add(tableRange, 17, 3);
            visitTable.Borders.InsideLineStyle = visitTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            visitTable.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

            Word.Range cellRange;

            visitTable.Cell(1, 1).Merge(visitTable.Cell(1, 2)); //Объединение ячеек
            visitTable.Cell(1, 1).Merge(visitTable.Cell(1, 2));

            cellRange = visitTable.Cell(1, 1).Range;
            Word.InlineShape img = cellRange.InlineShapes.AddPicture(AppDomain.CurrentDomain.BaseDirectory + "../../Resources/" + (selected.IsVisit ? "визит.png" : "мероприятие.png"));
            cellRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

            visitTable.Cell(2, 1).Merge(visitTable.Cell(2, 2));
            visitTable.Cell(2, 1).Merge(visitTable.Cell(2, 2));
            visitTable.Cell(3, 2).Merge(visitTable.Cell(3, 3));

            cellRange = visitTable.Cell(3, 1).Range;
            cellRange.Text = "Пациент";
            for (int i = 4; i <= 9; i++)
                visitTable.Cell(3, 1).Merge(visitTable.Cell(i, 1));

            visitTable.Cell(3, 2).Range.Text = current_patient.second_name + " " + current_patient.name + " " + current_patient.father_name;
            visitTable.Cell(4, 2).Range.Text = "Паспорт";
            visitTable.Cell(4, 3).Range.Text = current_patient.passport;
            visitTable.Cell(5, 2).Range.Text = "Амб. Карта";
            visitTable.Cell(5, 3).Range.Text = current_patient.card_number.ToString();
            visitTable.Cell(6, 2).Range.Text = "Дата рождения";
            visitTable.Cell(6, 3).Range.Text = current_patient.birthday.ToString("dd.MM.yyyy");
            visitTable.Cell(7, 2).Range.Text = "Пол";
            visitTable.Cell(7, 3).Range.Text = current_patient.sex==1?"Мужской":"Женский";
            visitTable.Cell(8, 2).Range.Text = "Адрес";
            visitTable.Cell(8, 3).Range.Text = current_patient.adress;
            visitTable.Cell(9, 2).Range.Text = "Мобильный телефон";
            visitTable.Cell(9, 3).Range.Text = current_patient.phone;



            visitTable.Cell(10, 1).Merge(visitTable.Cell(10, 2)); //Объединение ячеек
            visitTable.Cell(10, 1).Merge(visitTable.Cell(10, 2));

            visitTable.Cell(11, 2).Merge(visitTable.Cell(11, 3));
            cellRange = visitTable.Cell(11, 1).Range;
            cellRange.Text = "Доктор";
            visitTable.Cell(11, 2).Range.Text = current_doctor.second_name + " " + current_doctor.name + " " + current_doctor.father_name;
            visitTable.Cell(12, 2).Range.Text = "Мобильный телефон";
            visitTable.Cell(12, 3).Range.Text = current_doctor.phone;
            visitTable.Cell(13, 2).Range.Text = "Специальность";
            visitTable.Cell(13, 3).Range.Text = ToUpperFirst(current_doctor.specialization);
            visitTable.Cell(14, 2).Range.Text = "Должность";
            visitTable.Cell(14, 3).Range.Text = ToUpperFirst(current_doctor.post);


            for (int i = 12; i <= 14; i++)
                visitTable.Cell(11, 1).Merge(visitTable.Cell(i, 1));

            visitTable.Cell(15, 1).Merge(visitTable.Cell(15, 2)); //Объединение ячеек
            visitTable.Cell(15, 1).Merge(visitTable.Cell(15, 2));

            
            visitTable.Cell(16, 2).Merge(visitTable.Cell(16, 3));
            visitTable.Cell(16, 1).Merge(visitTable.Cell(17, 1));

            cellRange = visitTable.Cell(16, 1).Range;
            cellRange.Text = selected.IsVisit?"Прием":"Мероприятие";
            visitTable.Cell(16, 2).Range.Text = selected.Date;
            visitTable.Cell(17, 2).Range.Text = "Тип";
            if(selected.IsVisit)
            {
                visit current_event = hospitalEntities.Context.visit.Where(x=>x.id==selected.Id).First();
                visitTable.Cell(17, 3).Range.Text = current_event.type;
                Word.Paragraph resultParagraph = document.Paragraphs.Add();
                resultParagraph.Range.Text = ToUpperFirst(current_event.result);
                resultParagraph.Range.set_Style("Слабая ссылка");
            }
            else
            {
                healingevent current_event = hospitalEntities.Context.healingevent.Where(x => x.id == selected.Id).First();
                visitTable.Cell(17, 3).Range.Text = current_event.type;
                Word.Paragraph resultParagraph = document.Paragraphs.Add();
                resultParagraph.Range.Text = "Результат: "+current_event.result;
                resultParagraph.Range.set_Style("Слабая ссылка");
                resultParagraph.Range.InsertParagraphAfter(); // Добавляет разрыв абзаца
                resultParagraph = document.Paragraphs.Add();
                if (!current_event.recommendation.StartsWith("рек"))
                    current_event.recommendation = "рекомендации: " + current_event.recommendation;
                resultParagraph.Range.Text = ToUpperFirst(current_event.recommendation);
            }
        application.Visible = true;
        }
    }
}
