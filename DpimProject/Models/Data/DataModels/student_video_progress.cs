using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace DpimProject.Models.Data.DataModels
{
    [Table("student_video_progress")]
    public class student_video_progress //Create 1/16/2021 3:33:55 PM
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? id { get; set; }

        public int? student_id { get; set; }

        [MaxLength(10)]
        public string course_id { get; set; }

        public int? course_lesson_id { get; set; }

        public string video_path { get; set; }

        public decimal? video_position { get; set; }

        public decimal? video_progress { get; set; }

        public int? create_by { get; set; }

        public DateTime? create_at { get; set; }

        public int? update_by { get; set; }

        public DateTime? update_dt { get; set; }

    }
}