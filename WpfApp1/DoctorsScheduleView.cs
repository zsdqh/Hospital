using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class DoctorsScheduleView
    {
        public static String EngToRus(String name)
        {
            //List<String> rus = new List<string> { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота" };
            List<String> rus = new List<string> { "ПН", "ВТ", "СР", "ЧТ", "ПТ", "СБ" };
            List<String> eng = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
            return rus[eng.IndexOf(name)];
        }
        public int Id { get; set; }
        public String EngName { get; set; }
        public String Name { get { return EngToRus(EngName); } set { this.EngName = value; } }
        public string Text { get; set; }
        public DoctorsScheduleView(List<String> day, String name, int id)
        {
            Id = id;
            EngName = name;
            if (day == null)
            {
                Text = "-";
                return;
            }
            if(day.Count(x=>x==null)==2)
            {
                Text = day[0]+" - " + day[1];
            }
            else
            {
                Text = String.Format("чет {0}\nнечет {1}", day[0] + " - " + day[1], day[2] + " - " + day[3]);
            }

        }
    }
}