using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("student_forget_password_token")]
    public class student_forget_password_token
    {
        public int? id { get; set; }
        public int? user_id { get; set; }
        public string token { get; set; }
        public DateTime? create_at { get; set; }

        public DateTime? update_at { get; set; }

        public int? update_by { get; set; }

        public int? create_by { get; set; }
        public int? is_use { get; set; }
    }
}