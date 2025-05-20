using Microsoft.Office.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
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
    public class DoctorView
    {
        public doctor doctor {  get; set; }
        public String start {  get; set; }
        public String end { get; set; }
        public int ind {  get; set; }
        public String Text
        {
            get
            {
                String FIO = doctor.second_name + " " + doctor.name[0] + "." + doctor.father_name[0]+".";
                return FIO + " " + start + "-" + end;
            }
        }
        public DoctorView(doctor d, String start, String end, int ind)
        {
            doctor = d;
            this.start = start;
            this.end = end;
            this.ind = ind;
        }
    }
    public partial class PriemPage : Page, IEventHandler
    {
        private VisitService parent;
        public PriemPage(VisitService parent)
        {
            InitializeComponent();
            SpecCombo.ItemsSource = hospitalEntities.Context.doctor.Select(x => x.specialization).ToHashSet().ToList();
            this.parent = parent;
            SpecCombo.Text = "терапевт";
        }
        public void SomethingChanged()
        {
            FIlterDoctors(SpecCombo, null);
        }


        private void FIlterDoctors(object sender, SelectionChangedEventArgs e) 
        {
            // Функция фильтрации врачей
            List<doctor> items = hospitalEntities.Context.doctor.Where(x => x.specialization == (string)SpecCombo.SelectedItem).ToList(); //Все доктора нужной специализации
            List<DoctorView> available = new List<DoctorView>(); // Список свободных для записи врачей
            foreach(doctor d in items)
            {
                DoctorsSchedule ds;
                JSONworker.Context.TryGetValue(d.id, out ds); //Получение расписания врачей
                if (ds == null)
                {
                    continue; // Если для врача не задано расписание, то пропускаем его
                }
                if (parent.dateBox.SelectedDate == null)
                {
                    parent.dateBox.SelectedDate = DateTime.Now;
                }
                CultureInfo culture = new CultureInfo("en-US");
                DateTime wanted = (DateTime)parent.dateBox.SelectedDate;
                wanted = wanted.AddHours((double)parent.hourBox.Value);
                wanted = wanted.AddMinutes((double)parent.minuteBox.Value); // Получение желаемой даты приёма
                Time wanted_time = new Time(wanted.ToString("HH:mm"));
                List<String> day = ds[wanted.ToString("dddd", culture)]; // Получение расписания за выбранный день
                Time start, end;
                if (day == null) // Если врач в этот день не работает
                {
                    continue;
                }
                if (day[2]==null) // Если расписание не зависит от четности
                {
                    start = new Time(day[0]);
                    end = new Time(day[1]);
                }
                else // Если расписание зависит от четности
                {
                    if(wanted.Day%2==0) // Четная дата
                    {
                        start = new Time(day[0]);
                        end = new Time(day[1]);
                    }
                    else // Нечетная дата
                    {
                        start = new Time(day[2]);
                        end = new Time(day[3]);
                    }
                }
                bool flag = false; // Флаг того, свободен ли врач на заданное время
                if(start.CompareTo(wanted_time)<=0 && end.CompareTo(wanted_time)>=0)
                {
                    int ind = -1;  // номер талона
                    List<String> tickets;
                    ds.Tickets.TryGetValue(Time.DateToInt(wanted), out tickets); // Получение всех записей на выбранную дату
                    if(tickets==null)
                    {
                        tickets = new List<String>();
                    }
                    List<Time> time_tickets = new List<Time>();// Временное хранилище записей на прием
                    foreach (String s in tickets)
                        time_tickets.Add(new Time(s));
                    time_tickets.Sort(); // Сортировка времени записей по возрастанию
                    tickets = new List<string>();
                    foreach(Time t in time_tickets)
                        tickets.Add(t.ToString());
                    tickets = tickets.ToHashSet().ToList(); // Удаление дублирующихся записей, если такие имеются
                    for(int i=0;i<tickets.Count-1;i++)
                    {
                        if ((new Time(tickets[i])+ds.TimeSpent).CompareTo(wanted_time)<=0 && (wanted_time+ds.TimeSpent).CompareTo(new Time(tickets[i+1]))<=0)
                        {// Можно ли записать пациента на промежуток между другими приемами
                            flag = true; // Изменяем флаг на true, врач свободен
                            ind = i + 1;
                            break;
                        }
                    }
                    if (tickets.Count == 0 || (wanted_time + ds.TimeSpent).CompareTo(new Time(tickets[0])) <= 0)  
                    {
                        // Можно ли записать перед всеми приёмами
                        flag = true;
                        ind = 0; // 0 значит первый талон
                    }
                    else if (((new Time(tickets.Last())) + ds.TimeSpent).CompareTo(wanted_time) <= 0)
                    {
                        // Можно ли записать после всех приемов
                        flag = true;
                        ind = -2; // -2 значит последний талон
                    }

                    if (flag) // Добавляем врача, если он свободен на выбранное время
                    {
                        available.Add(new DoctorView(d, start.ToString(), end.ToString(), ind));
                    }
                }
            }
            doctorsAvailable.ItemsSource = available; // Обновляем список врачей
        }

        private void Zapisat(object sender, RoutedEventArgs e)
        {
            if (parent.hourBox.Value == null || parent.minuteBox.Value == null || parent.dateBox.SelectedDate == null || doctorsAvailable.SelectedItem == null)
                return; // Если какое-либо поле не заполненно(не выбрано), то выйти из функции
            int hour = (int)parent.hourBox.Value; // Получение данных из элементов управления
            int minute = (int)parent.minuteBox.Value;
            DateTime prefered_date = (DateTime)parent.dateBox.SelectedDate;
            prefered_date = prefered_date.AddHours(hour);
            prefered_date = prefered_date.AddMinutes(minute);
            if(prefered_date<=DateTime.Now) 
            { // Обработка попытки записи пациента на уже прошедшую дату
                MessageBox.Show("Дата уже прошла");
                return;
            }
            DoctorView dv = (doctorsAvailable.SelectedItem as DoctorView); // Получение выбранного доктора из списка докторов
            doctor d = dv.doctor;
            DoctorsSchedule ds;
            JSONworker.Context.TryGetValue(d.id, out ds); // Загрузка расписания доктора
            CultureInfo culture = new CultureInfo("en-US"); // Переменная для получения даты в нужном формате
            List<String> day; // Записи на прием к этому врачу на эту дату
            if (!ds.Tickets.TryGetValue(Time.DateToInt(prefered_date), out day))
            { // Если записей на этот день еще нет, то создать новый список
                day = new List<String>();
                ds.Tickets.Add(Time.DateToInt(prefered_date), day);
            }
            int talon = dv.ind; // получение номера талона, рассчитанного для каждого врача во время фильтрации
            if (talon==-2)
            { // Записать в конец очереди
                day.Add(prefered_date.ToString("HH:mm"));
            }
            else
            { // Добавить талон в список
                day.Insert(talon, prefered_date.ToString("HH:mm"));
            }
            visit v = new visit(); // Новое посещение к врачу
            v.id = hospitalEntities.Context.visit.ToList().OrderBy(x => x.id).Last().id + 1; // Присвоение id
            v.patient_id = parent._current_patient.id;
            v.patient = parent._current_patient;
            v.doctor_id = d.id;
            v.doctor = d;
            // Если этот пациент уже был на приеме этого врача, то приём вторичный, иначе - первичный
            v.type = hospitalEntities.Context.visit.ToList().Any(x=>x.doctor_id==d.id && x.patient_id==parent._current_patient.id) ? "вторичный" : "первичный";
            v.date = prefered_date;
            // Поскольку приём еще не был проведен, то результат пока null, а статус посещения false, то есть не посещено
            v.result = null;
            v.visited = false;
            hospitalEntities.Context.visit.Add(v); // Добавление записи в БД
            hospitalEntities.Context.SaveChanges();
            JSONworker.SaveChanges(); // Сохранение талонов
            SomethingChanged(); // Вызов функции обновления окна
            MessageBox.Show("Успешно записано!");
        }
    }
}
