﻿using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для Meropr.xaml
    /// </summary>
    public partial class MeroprPage : Page, IEventHandler
    {
        private VisitService parent;

        public MeroprPage(VisitService parent)
        {
            InitializeComponent();
            this.parent = parent;
            SpecCombo.ItemsSource = hospitalEntities.Context.doctor.Select(x => x.specialization).ToHashSet().ToList();
            SpecCombo.Text = "медсестра";
            TypeCombo.ItemsSource = hospitalEntities.Context.healingevent.Select(x => x.type).ToHashSet().ToList();
            TypeCombo.Text = "лабораторное исследование";


        }
        public void SomethingChanged()
        {
            SpecCombo_SelectionChanged(SpecCombo, null);
        }

        private void SpecCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (parent.hourBox.Value == null || parent.minuteBox.Value == null || parent.dateBox.SelectedDate == null || doctorsAvailable.SelectedItem == null || String.IsNullOrEmpty(NameBox.Text))
                return;
            int hour = (int)parent.hourBox.Value;
            int minute = (int)parent.minuteBox.Value;
            DateTime prefered_date = (DateTime)parent.dateBox.SelectedDate;
            prefered_date = prefered_date.AddHours(hour);
            prefered_date = prefered_date.AddMinutes(minute);
            if (prefered_date <= DateTime.Now)
            {
                MessageBox.Show("Дата уже прошла");
                return;
            }
            DoctorView dv = (doctorsAvailable.SelectedItem as DoctorView);
            doctor d = dv.doctor;
            DoctorsSchedule ds;
            JSONworker.Context.TryGetValue(d.id, out ds);
            CultureInfo culture = new CultureInfo("en-US");
            List<String> day;
            if (!ds.Tickets.TryGetValue(Time.DateToInt(prefered_date), out day))
            {
                day = new List<String>();
                ds.Tickets.Add(Time.DateToInt(prefered_date), day);
            }
            if (dv.ind == -2)
            {
                day.Add(prefered_date.ToString("HH:mm"));
            }
            else
            {
                day.Insert(dv.ind, prefered_date.ToString("HH:mm"));
            }
            healingevent hv = new healingevent();
            hv.id = hospitalEntities.Context.healingevent.ToList().OrderBy(x => x.id).Last().id + 1;
            hv.patient_id = parent._current_patient.id;
            hv.patient = parent._current_patient;
            hv.doctor_id = d.id;
            hv.doctor = d;
            hv.date = prefered_date;
            hv.result = null;
            hv.recommendation = null;
            hv.visited = false;
            hv.type = TypeCombo.Text;
            hv.name = NameBox.Text;

            hospitalEntities.Context.healingevent.Add(hv);
            hospitalEntities.Context.SaveChanges();
            JSONworker.SaveChanges();
            SomethingChanged();
            MessageBox.Show("Успешно записано!");
        }
    }
}
