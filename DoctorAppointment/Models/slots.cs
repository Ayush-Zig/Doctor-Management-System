using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoctorAppointment.Models
{
    public class slots
    {
        public int SlotId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Capacity { get; set; }

        public List<int> days { get; set; } = new List<int>(); // Initialize to prevent null reference

        public List<int> SlotIds { get; set; } = new List<int>();
    }
}