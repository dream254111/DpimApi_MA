using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace DpimProject.Models.Data.DataModels
{
    [Table("course_exam_setting")]
    public class course_exam_setting //Create 12/2/2020 8:27:21 PM
    {


        [Key]
        [Column(Order = 0)]
        public int? course_id { get; set; }

        [MaxLength(1)]
        public string random_answer { get; set; }

        [MaxLength(1)]
        public string course_answer { get; set; }

        public int? percent_pass { get; set; }

        public int? type_of_exam { get; set; }

        [Key]
        [Column(Order =1)]
        public int? exam_id { get; set; }

    }
}