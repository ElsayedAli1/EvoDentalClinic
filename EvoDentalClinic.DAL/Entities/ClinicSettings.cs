using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.DAL.Entities
{
    public class ClinicSettings
    {
        public int Id { get; set; }

        public string WorkDays { get; set; } = "Saturday,Sunday,Monday,Tuesday,Wednesday,Thursday";
        public TimeOnly OpenTime { get; set; } = new TimeOnly(16, 0);
        public TimeOnly CloseTime { get; set; } = new TimeOnly(21, 0);
        public int SlotMinutes { get; set; } = 30;
    }

}
