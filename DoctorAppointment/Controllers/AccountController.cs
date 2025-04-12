using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DoctorAppointment.Models;

namespace DoctorAppointment.Controllers
{
    

    public class AccountController : Controller
    {
        

        private readonly string strcon = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        // GET: Account

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Login(UserModel model)
        {
            using (SqlConnection conn = new SqlConnection(strcon))
            {
                string query = "SELECT * FROM Users WHERE Email=@UserName AND Password=@Password";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserName", model.UserName);
                    cmd.Parameters.AddWithValue("@Password", model.Password);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        // Store the UserName in the session after login
                        Session["UserName"] = reader["UserName"].ToString();
                        Session["Email"] = reader["Email"].ToString();
                       
                        Session["PhoneNumber"] = reader["PhoneNumber"].ToString();
                        Session["UserId"] = reader["UserId"].ToString();

                        FormsAuthentication.SetAuthCookie(reader["UserName"].ToString(), false);

                        Session["RoleId"] = (int)reader["RoleId"];
                        int RoleId = (int)Session["RoleId"];
                        int UserId = (int)reader["UserId"];

                        reader.Close();

                        if (RoleId == 1) 
                        {
                            string query2 = "SELECT DoctorID, FirstName FROM Doctors WHERE UserId = @UserId";

                            using (SqlCommand cmd2 = new SqlCommand(query2, conn))
                            {
                                cmd2.Parameters.AddWithValue("@UserId", UserId);
                                SqlDataReader reader2 = cmd2.ExecuteReader();

                                if (reader2.Read())
                                {
                                    Session["DoctorId"] = reader2["DoctorID"].ToString();
                                    Session["DoctorName"] = reader2["FirstName"].ToString();
                                }
                                reader2.Close();
                            }
                            return RedirectToAction("Dashboard", "Doctor");
                        }
                        else
                        {
                            string query3 = "SELECT PatientID, FirstName , DateOfBirth FROM Patients WHERE UserId = @UserId";

                            using (SqlCommand cmd3 = new SqlCommand(query3, conn))
                            {
                                cmd3.Parameters.AddWithValue("@UserId", UserId);
                                SqlDataReader reader3 = cmd3.ExecuteReader();

                                if (reader3.Read())
                                {
                                    Session["PatientId"] = reader3["PatientID"].ToString();
                                    Session["PatientName"] = reader3["FirstName"].ToString();
                                    Session["DateOfBirth"] = reader3["DateOfBirth"] != DBNull.Value ?
                                                    Convert.ToDateTime(reader3["DateOfBirth"]).ToString("yyyy-MM-dd") : null;
                                  
                                }
                                reader3.Close();
                            }
                            return RedirectToAction("Dashboard", "Patient");
                        }
                    }
                }
            }
            return View();
        }







        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(UserModel model)
        {
        

            object UserId;
            using (SqlConnection conn = new SqlConnection(strcon))
            {
                conn.Open();

                string UserQuery = @"INSERT INTO Users (UserName, Password, Email, PhoneNumber, RoleId) 
                             VALUES (@UserName, @Password, @Email, @PhoneNumber, @RoleId); 
                             SELECT SCOPE_IDENTITY()";

                using (SqlCommand cmd = new SqlCommand(UserQuery, conn))
                {
                    
                    cmd.Parameters.AddWithValue("@UserName", model.UserName);
                    cmd.Parameters.AddWithValue("@Password", model.Password);
                    cmd.Parameters.AddWithValue("@Email", model.Email);
                    cmd.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);
                    cmd.Parameters.AddWithValue("@RoleId", 2); 

                    UserId = cmd.ExecuteScalar();  
                }

                // Insert into Patients tabl
                string PatientQuery = @"INSERT INTO Patients (FirstName, Email, DateOfBirth, Phone,UserId, RegisterDT) 
                        VALUES (@FirstName, @Email, @DateOfBirth, @Phone, @UserId, @RegisterDT)";


                using (SqlCommand cmd = new SqlCommand(PatientQuery, conn))
                {
                  
                    cmd.Parameters.AddWithValue("@FirstName", model.UserName);
                    cmd.Parameters.AddWithValue("@Email", model.Email);
                    cmd.Parameters.AddWithValue("@DateOfBirth", model.DateOfBirth);
                    cmd.Parameters.AddWithValue("@Phone", model.PhoneNumber);
                    cmd.Parameters.AddWithValue("@UserId", UserId);  
                    cmd.Parameters.AddWithValue("@RegisterDT", DateTime.Now);
                    

                    int a = cmd.ExecuteNonQuery();

                    if (a > 0)
                    {
                        
                        Session["UserName"] = model.UserName;
                        Session["Email"] = model.Email;
                        Session["Phone"] = model.PhoneNumber;
                        Session["DateOfBirth"] = model.DateOfBirth;
                        Session["UserId"] = model.UserId;


                        ViewBag.SuccessMessage = "You are registered successfully!";
                        
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Something Went Wrong!";
                    }
                }
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }









































        [HttpGet]
public ActionResult UpdateRegister(int UserId = 0)
{
    UserModel model = new UserModel();
    if (UserId > 0)
    {
        using (SqlConnection conn = new SqlConnection(strcon))
        {
            conn.Open();
            // Query to get user details
            string UserQuery = "SELECT u.UserName, u.Email, u.PhoneNumber, p.DateOfBirth " +
                               "FROM Users u " +
                               "INNER JOIN Patients p ON u.UserId = p.UserId " +
                               "WHERE u.UserId = @UserId";

            using (SqlCommand cmd = new SqlCommand(UserQuery, conn))
            {
                cmd.Parameters.AddWithValue("@UserId", UserId);
                SqlDataReader reader = cmd.ExecuteReader();
                
                if (reader.Read())
                {
                    model.UserName = reader["UserName"].ToString();
                    model.Email = reader["Email"].ToString();
                    model.PhoneNumber = reader["PhoneNumber"].ToString();
                    model.DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]);
                }
            }
        }
    }
    return View(model);
}

        [HttpPost]
 
        public ActionResult UpdateRegister(UserModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Please correct the errors in the form.";
                return View(model); // Return the same view to show validation errors
            }
            using (SqlConnection conn = new SqlConnection(strcon))
            {
                conn.Open();

                // Update query for Users table
                string UserQuery = @"UPDATE Users 
                             SET UserName = @UserName, Email = @Email, PhoneNumber = @PhoneNumber 
                             WHERE UserId = @UserId";

                using (SqlCommand cmd = new SqlCommand(UserQuery, conn))
                {
                    // Ensure all parameters are being added
                    cmd.Parameters.AddWithValue("@UserId", model.UserId);
                    cmd.Parameters.AddWithValue("@UserName", model.UserName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", model.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber ?? (object)DBNull.Value);

                    int rowsAffected = cmd.ExecuteNonQuery(); // Update the Users table

                    if (rowsAffected > 0)
                    {
                        // Update query for Patients table
                        string PatientQuery = @"UPDATE Patients 
                                        SET DateOfBirth = @DateOfBirth 
                                        WHERE UserId = @UserId";

                        using (SqlCommand cmd2 = new SqlCommand(PatientQuery, conn))
                        {
                            // Add parameters for Patients table
                            cmd2.Parameters.AddWithValue("@UserId", model.UserId);
                            cmd2.Parameters.AddWithValue("@DateOfBirth", model.DateOfBirth);

                            rowsAffected = cmd2.ExecuteNonQuery(); // Update the Patients table

                            if (rowsAffected > 0)
                            {
                                // Update session data
                                Session["UserName"] = model.UserName;
                                Session["Email"] = model.Email;
                                Session["DateOfBirth"] = model.DateOfBirth;
                                Session["PhoneNumber"] = model.PhoneNumber;

                                ViewBag.SuccessMessage = "Your profile has been updated successfully!";
                                return RedirectToAction("Profile", "Account"); // Redirect to a profile view or similar
                            }
                            else
                            {
                                ViewBag.ErrorMessage = "Failed to update patient details.";
                            }
                        }
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Failed to update user details.";
                    }
                }
            }
            return View(model);
        }


    }
}




