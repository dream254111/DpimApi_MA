using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DpimProject.Models.Data.DataModels
{
    [Table("course_exam_form")]
    public class course_exam_form //Create 10/23/2020 4:48:15 PM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(10)]
        public string course_exam_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [MaxLength(10)]
        public string course_id { get; set; }

        [MaxLength(10)]
        public string status { get; set; }

        public decimal? pass_percent { get; set; }

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