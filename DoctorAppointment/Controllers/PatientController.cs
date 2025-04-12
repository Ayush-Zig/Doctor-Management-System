using DoctorAppointment.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Mvc;

namespace DoctorAppointment.Controllers
{
    [Authorize]
    public class PatientController : Controller
    {
        private readonly string strcon = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        // GET: Patient
        public ActionResult Dashboard()
        {
            ViewBag.UserName = Session["UserName"];
            ViewBag.Email = Session["Email"];
            ViewBag.DateOfBirth = Session["DateOfBirth"];
            ViewBag.PhoneNumber = Session["PhoneNumber"];
           
            ViewBag.PatientId = Session["PatientId"];
            return View();
        }


        public ActionResult MyAppointment()
        {
            List<AppointmentModel> Aptlist = new List<AppointmentModel>();

            using (SqlConnection con = new SqlConnection(strcon))
            {
                string query = @"SELECT t1.*, t2.FirstName as DoctorName, t3.StatusName as AptStatus, t4.StartTime, t4.EndTime
                FROM Appointments t1 JOIN Doctors t2 ON t1.DoctorID = t2.DoctorID 
                join AppointmentStatus t3 on t1.StatusID = t3.StatusID
				join Slots t4  on t1.SlotID = t4.SlotId 
				WHERE PatientID = @PatientID";
             

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    object PatientId = Session["PatientId"];

                    cmd.Parameters.AddWithValue("@PatientId", PatientId);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        AppointmentModel apt = new AppointmentModel();
                        apt.AppointmentID = (int)reader["AppointmentID"];
                        apt.StartTime = (TimeSpan)reader["StartTime"];
                        apt.EndTime = (TimeSpan)reader["EndTime"];
                        apt.AptStatus = reader["AptStatus"].ToString();
                        apt.PaymentScreenshotName = reader["PaymentScreenshot"].ToString();
                        apt.AppointmentDate = (DateTime)reader["AppointmentDate"];

                        Aptlist.Add(apt);
                    }
                }
            }

            return View(Aptlist);
        }
    }
}