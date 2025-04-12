using DoctorAppointment.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace DoctorAppointment.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly string strcon = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult BookAppointment(int DoctorId = 2)
        {
            using (SqlConnection con = new SqlConnection(strcon))
            {
                string query = "SELECT * FROM AppointmentFees WHERE DoctorID = @DoctorID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@DoctorID", DoctorId);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        
                        ViewBag.AppointmentFees = reader["FeeAmount"];
                    }
                }
            }

            return View();
        }

        [HttpPost]
        public ActionResult BookAppointment(AppointmentModel model)
        {
            string FileName = "";
            if (model.PaymentScreenshot != null)
            {
                FileName = Path.GetFileName(model.PaymentScreenshot.FileName);
                string FilePath = Server.MapPath("/content/Images/paymentscreenshot/" + FileName);
                

                model.PaymentScreenshot.SaveAs(FilePath);
            }

            using (SqlConnection con = new SqlConnection(strcon))
            {
                object PatientID = Session["PatientId"];
                string query = @"
    INSERT INTO Appointments 
    (DoctorID, PatientID, SlotID, AppointmentDate, StatusID, Fee, PaymentStatusID, PaymentScreenshot)
    VALUES
    (@DoctorID, @PatientID, @SlotID, @AppointmentDate, @StatusID, @Fee, @PaymentStatusID, @PaymentScreenshot)";
                /*"INSERT INTO Appointments VALUES(@DoctorID,@PatientID,@SlotID, @AppointmentDate, @StatusID, @Fee, @PaymentStatusID, @PaymentScreenshot)*//*"*/;
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@DoctorID", 2);
                    cmd.Parameters.AddWithValue("@PatientID", PatientID);
                    cmd.Parameters.AddWithValue("@SlotID", model.SlotId);
                    cmd.Parameters.AddWithValue("@AppointmentDate", model.AppointmentDate);
                    cmd.Parameters.AddWithValue("@StatusID", 1);
                    cmd.Parameters.AddWithValue("@Fee", model.Fee);
                    cmd.Parameters.AddWithValue("@PaymentStatusID", 1);
                    cmd.Parameters.AddWithValue("@PaymentScreenshot", FileName);

                    con.Open();

                    int a = cmd.ExecuteNonQuery();

                    if (a > 0)
                    {
                        TempData["SuccessMessage"] = "Appointment booked successfully!";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Something Went Wrong!";
                    }
                }
            }
            return RedirectToAction("MyAppointment", "Patient");
        }


        public JsonResult GetSlot(int DayOfWeek)
        {
            List<slots> SlotList = new List<slots>();

            using (SqlConnection con = new SqlConnection(strcon))
            {
                string query = "SELECT rs.SlotID, s.StartTime, s.EndTime FROM RecurringSlots rs JOIN Slots s ON rs.DayOfWeek = @DayOfWeek WHERE s.SlotId = rs.SlotID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@DayOfWeek", DayOfWeek);
                    con.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        slots slot = new slots
                        {
                            SlotId = (int)reader["SlotID"],
                            StartTime = (TimeSpan)reader["StartTime"],
                            EndTime = (TimeSpan)reader["EndTime"]
                        };

                        SlotList.Add(slot);
                    }
                }
            }

            var slotResponse = SlotList.Select(slot => new
            {
                SlotId = slot.SlotId,
                StartTime = slot.StartTime.ToString(@"hh\:mm"),
                EndTime = slot.EndTime.ToString(@"hh\:mm")
            }).ToList();

            return Json(slotResponse, JsonRequestBehavior.AllowGet);
        }

    }
}