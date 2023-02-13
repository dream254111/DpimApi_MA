using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace DpimProject.Models.Data.DataModels
{
    [Table("student_course")]
    public class student_course //Create 10/23/2020 4:55:58 PM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(10)]
        public string course_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [MaxLength(10)]
        public string user_id { get; set; }

        [MaxLength(10)]
        public string learn_status { get; set; }

        public DateTime? start_dt { get; set; }

        public DateTime? end_dt { get; set; }

        [MaxLength(255)]
        public string add_by { get; set; }

        public DateTime? add_dt { get; set; }

        [MaxLength(255)]
        public string edit_by { get; set; }

        public DateTime? edit_dt { get; set; }

        [MaxLength(255)]
        public string del_by { get; set; }

        public DateTime? del_dt { get; set; }

    }
}