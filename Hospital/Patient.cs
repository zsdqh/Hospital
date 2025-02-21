using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital
{
    internal class Patient
    {
        public int Id { get; set; }
        public string Photo {  get; set; }
        public string Name { get; set; }
        public string Second_name { get; set; }
        public string Father_name { get; set; }
        public string Passport { get; set; }
        public DateOnly BirthDay { get; set; }
        public int Sex { get; set; }
        public string Adress { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int Card_number { get; set; }
        public DateOnly Card_date { get; set; }
        public DateTime Last_visit { get; set; }
        public DateTime Next_visit { get; set; }
        public int Policy_number { get; set; }
        public DateOnly Policy_end { get; set; }
        public string Diagnosis { get; set; }
        public string History { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
