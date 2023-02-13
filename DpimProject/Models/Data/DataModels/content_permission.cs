using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DpimProject.Models.Data.DataModels
{
    [Table("content_permission")]
    public class content_permission
    {
        [Key]
        [Column(Order = 0)]
        public int id { get; set; }
        public string course_id { get; set; }
        public int? video_id { get; set; }
        public int student_type_permission { get; set; }
    }
}