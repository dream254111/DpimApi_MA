using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("course_exam_answer")]
    public class course_exam_answer //Create 1/15/2021 8:29:20 PM
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? course_exam_id { get; set; }

        [Key]
        [Column(Order = 2)]
        [MaxLength(10)]
        public string course_id { get; set; }

        public int? correct { get; set; }

        public int? order { get; set; }

        [MaxLength(-1)]
        public string answer { get; set; }

    }
}