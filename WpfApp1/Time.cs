using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class Time:IComparable<Time>
    {
        public int Hours { get; set; }
        public int Minutes {  get; set; }
        public Time(int h, int m)
        {
            Hours = h; Minutes = m;
        }
        public Time(String s)
        {
            var splited = s.Split(':');
            Hours= int.Parse(splited[0]);
            Minutes= int.Parse(splited[1]);
        }
        public Time()
        {
            Hours = -1;
            Minutes = -1;
        }
        public override string ToString()
        {
            if (Hours==-1 || Minutes==-1)
            {
                return null;
            }
            return String.Format("{0:00}:{1:00}", Hours, Minutes);
        }

        public int CompareTo(Time other)
        {
            if (this.Hours == other.Hours)
            {
                return this.Minutes.CompareTo(other.Minutes);
            }
            return this.Hours.CompareTo(other.Hours);
        }
    }
}
