using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class DoctorsSchedule
    {
        public List<String> Monday { get; set; }
        public List<String> Tuesday { get; set; }
        public List<String> Wednesday { get; set; }
        public List<String> Thursday { get; set; }
        public List<String> Friday { get; set; }
        public List<String> Saturday { get; set; }
        public Dictionary<int, List<String>> Tickets { get; set; }
        public int TimeSpent { get; set; }

        private List<String> CreateWeekDay( Random rnd)
        {
            if (rnd.Next(1,5)==4)
            {
                return null;
            }
            List<String> hours= new List<String>() { null, null, null, null};
            hours[0] = (new Time(rnd.Next(6, 18), rnd.Next(1,3)==1?30:0)).ToString();
            Time t2 = new Time(hours[0]);
            t2.Hours += rnd.Next(4, 24 - t2.Hours + 1);
            hours[1] = t2.ToString();
            if(rnd.Next(1, 4)==3)
            {
                Time t1 = new Time(hours[0]);
                if (t1.Hours>=12)
                    {
                    t1.Hours -= 4;
                    hours[2] = t1.ToString();
                    t1.Hours += 8;
                    hours[3] = t1.ToString();
                }
                else
                {
                    t1.Hours += 4;
                    hours[2] = t1.ToString();
                    t1.Hours += 4;
                    hours[3] = t1.ToString();
                }
            }
            return hours;
        }
        public DoctorsSchedule() { }
        public List<String> this[String day]
        {
            get
            {
                switch (day)
                {
                    case "Monday": return Monday;
                    case "Tuesday": return Tuesday;
                    case "Wednesday": return Wednesday;
                    case "Thursday": return Thursday;
                    case "Friday": return Friday;
                    case "Saturday": return Saturday;
                    default: return null;
                }
            }
            set
            {
                switch (day)
                {
                    case "Monday": Monday = value; break;
                    case "Tuesday": Tuesday = value; break;
                    case "Wednesday": Wednesday = value; break;
                    case "Thursday": Thursday = value; break;
                    case "Friday": Friday = value; break;
                    case "Saturday": Saturday = value; break;
                }
            }
        }



        public DoctorsSchedule(Random r)
        {
            Monday = CreateWeekDay(r);
            Tuesday = CreateWeekDay(r);
            Wednesday = CreateWeekDay(r);
            Thursday = CreateWeekDay(r);
            Friday = CreateWeekDay(r);
            Saturday = CreateWeekDay(r);
            Tickets = new Dictionary<int, List<string>>();
            TimeSpent = (new Random()).Next(1, 4) * 5;

        }
        public DoctorsSchedule(List<String> monday, List<String> tuesday, List<String> wednesday, List<String> thursday, List<String> friday, List<String> saturday, Dictionary<int, List<string>> tickets, int timeSpent)
        {
            Monday = monday;
            Tuesday = tuesday;
            Wednesday = wednesday;
            Thursday = thursday;
            Friday = friday;
            Saturday = saturday;
            Tickets = tickets;
            TimeSpent = timeSpent;
        }
    }
}
