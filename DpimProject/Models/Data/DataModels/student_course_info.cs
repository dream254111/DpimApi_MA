using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("student_course_info")]
    public class student_course_info //Create 1/15/2021 8:30:41 PM
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? id { get; set; }

        [Key]
        [Column(Order = 1)]
        [MaxLength(10)]
        public string course_id { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? student_id { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? course_lesson_id { get; set; }

        public int? cert_print_count { get; set; }

        public int? is_extend_study_time { get; set; }

        public DateTime? learning_startdate { get; set; }

        public DateTime? learning_enddate { get; set; }

        public int? is_deleted { get; set; }

        public int? created_by { get; set; }

        public DateTime? created_dt { get; set; }

        public int? update_by { get; set; }

        public DateTime? update_dt { get; set; }

        [MaxLength(10)]
        public string voucher_id { get; set; }
    }
}