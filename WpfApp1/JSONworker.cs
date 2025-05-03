using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace WpfApp1
{
    public  class JSONworker
    {
        public static Dictionary<int, DoctorsSchedule> current_data = null;
        public static Dictionary<int, DoctorsSchedule> Context
        {
            get
            {
                if (current_data == null)
                {

                    if(!File.Exists("doctors.json"))
                    {
                        current_data = new Dictionary<int, DoctorsSchedule>();
                        using( FileStream fs = new FileStream("doctors.json", FileMode.Create))
                        {
                            Random r = new Random();
                            foreach(int d in hospitalEntities.Context.doctor.Select(x=>x.id))
                            {
                                current_data.Add(d, new DoctorsSchedule(r));
                            }
                            JsonSerializer.Serialize<Dictionary<int, DoctorsSchedule>>(fs, current_data);
                        }

                    }
                    using (FileStream fs = new FileStream("doctors.json", FileMode.OpenOrCreate))
                    {
                        current_data = JsonSerializer.Deserialize<Dictionary<int, DoctorsSchedule>>(fs);
                    }
                }
                return current_data;
            }
        }
        public static void SaveChanges()
        {
            using (FileStream fs = new FileStream("doctors.json", FileMode.Create))
            {
                JsonSerializer.Serialize<Dictionary<int, DoctorsSchedule>>(fs, current_data);
            }
        }
    }
}
