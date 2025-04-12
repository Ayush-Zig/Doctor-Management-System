using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoctorAppointment.Models
{
    public class AppointmentModel
    {
        public int AppointmentID { get; set; }
        public int DoctorID { get; set; }
        public int PatientID { get; set; }
        public int SlotId { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public DateTime AppointmentDate { get; set; }
        public int StatusID { get; set; }
        public decimal Fee { get; set; }
        public int PaymentStatusID { get; set; }

        public HttpPostedFileBase PaymentScreenshot { get; set; }
        public string PaymentScreenshotName { get; set; }

        public string PatientName { get; set; }

        public string AptStatus { get; set; }
        public string PaymentStatus { get; set; }
    }
}