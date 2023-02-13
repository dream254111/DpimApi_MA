using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("video_status")]
    public class video_status //Create 12/6/2020 12:36:14 PM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(255)]
        public string filename { get; set; }

        [MaxLength(1)]
        public string status { get; set; }
        //public int? course_lesson_id { get; set; }

        //public int? course_id { get; set; }

    }
}