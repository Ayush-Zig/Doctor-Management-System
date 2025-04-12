using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DoctorAppointment.Models
{
    public class UserModel
    {
        public int UserId { get; set; }
        public int PatientId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string RoleId { get; set; }
     
        public DateTime DateOfBirth
        {
            get; set;

        }
        public string ProfileImagePath { get; set; }

        // For uploading files
        [DataType(DataType.Upload)]
        public HttpPostedFileBase ProfileImage { get; set; }
    }
}