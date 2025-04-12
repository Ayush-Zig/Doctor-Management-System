using DoctorAppointment.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoctorAppointment.Controllers
{
    [Authorize]
    public class DoctorController : Controller
    {
        private readonly string strcon = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        // GET: Doctor
        public ActionResult Dashboard()
        {
            ViewBag.UserName = Session["UserName"];
            ViewBag.Email = Session["Email"];
            return View();
        }
        [HttpGet]
        public ActionResult AppointmentList()
        {
            List<AppointmentModel> Aptlist = new List<AppointmentModel>();

            using (SqlConnection con = new SqlConnection(strcon))
            {
                //string query = @"SELECT t1.*, t2.Name as DoctorName, t3.Name as PatientName, t4.StatusName as AptStatus, t5.StatusName as PaymentStatus  FROM Appointments t1 JOIN Doctors t2 ON t1.DoctorID = t2.DoctorID 
                //JOIN Patients t3 on t1.PatientID = t3.PatientID join AppointmentStatus t4 on t1.StatusID = t4.StatusID 
                //JOIN PaymentStatus t5 on t1.PaymentStatusID = t5.StatusID 
                //";


                string query = @"SELECT t1.*, t2.FirstName as DoctorName, t5.FirstName as PatientName, t3.StatusName as AptStatus, t4.StartTime, t4.EndTime
                FROM Appointments t1 JOIN Doctors t2 ON t1.DoctorID = t2.DoctorID 
                JOIN Patients t5 on t1.PatientID = t5.PatientID
                join AppointmentStatus t3 on t1.StatusID = t3.StatusID
				join Slots t4  on t1.SlotId = t4.SlotId ";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        AppointmentModel apt = new AppointmentModel();
                        apt.AppointmentID = (int)reader["AppointmentID"];
                        apt.PatientName = reader["PatientName"].ToString();
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

        [HttpGet]
        public ActionResult CreateSlots()
        {
            List<slots> SlotList = new List<slots>();
            using (SqlConnection con = new SqlConnection(strcon))
            {
                string query = "SELECT * FROM Slots";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        slots newslot = new slots();
                        newslot.SlotId = (int)reader["SlotId"];
                        newslot.StartTime = (TimeSpan)reader["StartTime"];
                        newslot.EndTime = (TimeSpan)reader["EndTime"];
                        newslot.Capacity = (int)reader["Capacity"];

                        SlotList.Add(newslot);
                    }
                }
            }
            return View(SlotList);
        }

        //[HttpPost]
        //public ActionResult CreateSlots(slots model)
        //{
        //    List<slots> SlotList = new List<slots>();

        //    using (SqlConnection con = new SqlConnection(strcon))
        //    {
        //        string query = "INSERT INTO Slots VALUES(@StartTime, @EndTime, @Capacity)";
        //        using (SqlCommand cmd = new SqlCommand(query, con))
        //        {
        //            cmd.Parameters.AddWithValue("@StartTime", model.StartTime);
        //            cmd.Parameters.AddWithValue("@EndTime", model.EndTime);
        //            cmd.Parameters.AddWithValue("@Capacity", model.Capacity);
        //            con.Open();
        //            int a = cmd.ExecuteNonQuery();


        //            string query2 = "SELECT * FROM Slots";

        //            using (SqlCommand cmd2 = new SqlCommand(query2, con))
        //            {
        //                SqlDataReader reader = cmd2.ExecuteReader();

        //                while (reader.Read())
        //                {
        //                    slots newslot = new slots();
        //                    newslot.SlotId = (int)reader["SlotId"];
        //                    newslot.StartTime = (TimeSpan)reader["StartTime"];
        //                    newslot.EndTime = (TimeSpan)reader["EndTime"];
        //                    newslot.Capacity = (int)reader["Capacity"];

        //                    SlotList.Add(newslot);
        //                }
        //            }

        //            if (a > 0)
        //            {
        //                ViewBag.SuccessMsg = "Slot Created Successfully!";
        //                return View(SlotList);
        //            }
        //            else
        //            {
        //                ViewBag.ErrorMsg = "Something went wrong!";
        //                return View(SlotList);
        //            }
        //        }
        //    }
        //}
        [HttpPost]
        public ActionResult CreateSlots(slots model)
        {
            List<slots> SlotList = new List<slots>();

            using (SqlConnection con = new SqlConnection(strcon))
            {
                // Insert the new slot
                string insertQuery = "INSERT INTO Slots (StartTime, EndTime, Capacity) VALUES (@StartTime, @EndTime, @Capacity)";
                using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                {
                    cmd.Parameters.AddWithValue("@StartTime", model.StartTime);
                    cmd.Parameters.AddWithValue("@EndTime", model.EndTime);
                    cmd.Parameters.AddWithValue("@Capacity", model.Capacity);
                    con.Open();

                    int result = cmd.ExecuteNonQuery();
                    if (result <= 0)
                    {
                        ViewBag.ErrorMsg = "Failed to create the slot!";
                        return View(SlotList);
                    }
                }

                // Retrieve all slots
                string selectQuery = "SELECT * FROM Slots";
                using (SqlCommand cmd = new SqlCommand(selectQuery, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        slots newslot = new slots
                        {
                            SlotId = (int)reader["SlotId"],
                            StartTime = (TimeSpan)reader["StartTime"],
                            EndTime = (TimeSpan)reader["EndTime"],
                            Capacity = (int)reader["Capacity"]
                        };
                        SlotList.Add(newslot);
                    }
                }
            }

            ViewBag.SuccessMsg = "Slot Created Successfully!";
            return View(SlotList);
        }
        //[HttpPost]
        //public ActionResult AssignSlots(List<int> SlotIds, List<int> days)
        //{
        //    if (SlotIds == null || !SlotIds.Any())
        //    {
        //        TempData["ErrorMsg"] = "No slots selected!";
        //        return RedirectToAction("CreateSlots"); // Adjust based on your redirection needs
        //    }

        //    using (SqlConnection con = new SqlConnection(strcon))
        //    {
        //        con.Open();

        //        foreach (var slotId in SlotIds)
        //        {
        //            foreach (var day in days)
        //            {
        //                // Insert or update logic for slot-day mapping
        //                string query = "INSERT INTO SlotAssignments (SlotId, Day) VALUES (@SlotId, @Day)";
        //                using (SqlCommand cmd = new SqlCommand(query, con))
        //                {
        //                    cmd.Parameters.AddWithValue("@SlotId", slotId);
        //                    cmd.Parameters.AddWithValue("@Day", day);
        //                    cmd.ExecuteNonQuery();
        //                }
        //            }
        //        }
        //    }

        //    TempData["SuccessMsg"] = "Slots assigned successfully!";
        //    return RedirectToAction("CreateSlots"); // Adjust redirection as needed
        //}

        [HttpGet]
        public ActionResult Fees()
        {
            int FeeId = 0;
            int FeeAmount = 0;
            using (SqlConnection conn = new SqlConnection(strcon))
            {
                string query = "select * from AppointmentFees";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {

                        FeeId = Convert.ToInt32(reader["FeeID"]);
                        FeeAmount = Convert.ToInt32(reader["FeeAmount"]);
                    }


                }
            }
            ViewBag.FeeId = FeeId;
            ViewBag.FeeAmount = FeeAmount;


            return View();

        }

        [HttpPost]
        public ActionResult Fees(int FeeId, int FeesAmount)
        {

            if (FeeId == 0)
            {


                object DoctorId = Session["DoctorId"];
             
                using (SqlConnection conn = new SqlConnection(strcon))
                {

                    string query = "INSERT INTO AppointmentFees VALUES(@DoctorID, @FeeAmount)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {

                        cmd.Parameters.AddWithValue("@DoctorID", DoctorId);
                        cmd.Parameters.AddWithValue("@FeeAmount", FeesAmount);
                        conn.Open();
                        int a = cmd.ExecuteNonQuery();
                        if (a > 0)
                        {
                            TempData["SuccessMsg"] = "Fees Added Successfully!";
                            return RedirectToAction("Fees", "Doctor");
                        }
                        else
                        {
                            ViewBag.ErrorMsg = "Somthing Went Wrong!";
                            return View();
                        }



                    }

                }


            }
            else
            {

                using (SqlConnection con = new SqlConnection(strcon))
                {
                    string query = "Update AppointmentFees set FeeAmount = @FeeAmount where FeeID = @FeeID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@FeeID", FeeId);
                        cmd.Parameters.AddWithValue("@FeeAmount", FeesAmount);

                        con.Open();
                        int a = cmd.ExecuteNonQuery();
                        if (a > 0)
                        {
                            TempData["SuccessMsg"] = "Fees Updated Successfully!";
                            return RedirectToAction("Fees", "Doctor");
                        }
                        else
                        {
                            ViewBag.ErrorMsg = "Somthing Went Wrong!";
                            return View();
                        }


                    }
                }
            }
        }

        public ActionResult AssignSlots(slots model)
        {
            object DoctorId = Session["DoctorId"];
            using (SqlConnection con = new SqlConnection(strcon))
            {
                string query = "INSERT INTO RecurringSlots VALUES(@DoctorID, @DayOfWeek, @SlotID)";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    int a = 0;


                    foreach (var day in model.days)
                    {
                        foreach (var slotId in model.SlotIds)
                        {
                            cmd.Parameters.AddWithValue("@DoctorID", DoctorId);
                            cmd.Parameters.AddWithValue("@DayOfWeek", day);
                            cmd.Parameters.AddWithValue("@SlotID", slotId);
                            con.Open();
                            a = cmd.ExecuteNonQuery();
                            con.Close();
                            cmd.Parameters.Clear();
                        }
                    }

                    if (a > 0)
                    {
                        TempData["SuccessMsg"] = "Slot Assigned Successfully!";
                        return RedirectToAction("CreateSlots", "Doctor");
                    }
                    else
                    {
                        TempData["ErrorMsg"] = "Somthing Went Wrong!";
                    }
                }
            }
            return RedirectToAction("CreateSlots", "Doctor");
        }

        [HttpGet]
        public ActionResult DeleteSlot()
        {
            return View();
        }



        [HttpPost]
        public ActionResult DeleteSlot(int slotId)
        {
            using (SqlConnection con = new SqlConnection(strcon))
            {
                con.Open();

                // Delete related entries from RecurringSlots
                string deleteRecurringSlotsQuery = "DELETE FROM RecurringSlots WHERE SlotID = @SlotID";
                using (SqlCommand cmd = new SqlCommand(deleteRecurringSlotsQuery, con))
                {
                    cmd.Parameters.AddWithValue("@SlotID", slotId);
                    cmd.ExecuteNonQuery();
                }

                // Now delete the slot itself
                string deleteSlotQuery = "DELETE FROM Slots WHERE SlotId = @SlotID";
                using (SqlCommand cmd = new SqlCommand(deleteSlotQuery, con))
                {
                    cmd.Parameters.AddWithValue("@SlotID", slotId);
                    cmd.ExecuteNonQuery();
                }
            }

            TempData["SuccessMsg"] = "Slot deleted successfully!";
            return RedirectToAction("CreateSlots");
        }















        public JsonResult ChangeStatus(int appointmentId = 0, int statusId = 0)
        {
            if (appointmentId > 0 && statusId > 0)
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    string query = "UPDATE Appointments SET StatusID= @StatusID WHERE AppointmentID=@AppointmentID";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@StatusID", statusId);
                        cmd.Parameters.AddWithValue("@AppointmentID", appointmentId);

                        con.Open();

                        int a = cmd.ExecuteNonQuery();

                        if (a > 0)
                        {

                            return Json(new { appointmentId, statusId, success = true });
                        }
                        else
                        {
                            return Json(new { appointmentId, statusId, success = false });
                        }
                    }


                }
            }
            return Json(new { appointmentId, statusId, success = false });
        }




    }

}