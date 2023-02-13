using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("course_lesson_exercise")]
    public class course_lesson_exercise //Create 1/15/2021 8:28:38 PM
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
        public int? course_lesson_id { get; set; }

        public int? order { get; set; }

        [MaxLength(-1)]
        public string question { get; set; }

        [MaxLength(-1)]
        public string image { get; set; }

        [MaxLength(-1)]
        public string video { get; set; }

        [MaxLength(-1)]
        public string p_480 { get; set; }

        [MaxLength(-1)]
        public string p_720 { get; set; }

        [MaxLength(-1)]
        public string p_1080 { get; set; }

        [MaxLength(-1)]
        public string p_original { get; set; }

        [MaxLength(-1)]
        public string cover_video { get; set; }

        public int? is_answer_match { get; set; }

        public int? is_answer_choice { get; set; }

        public int? is_deleted { get; set; }

        public int? created_by { get; set; }

        public DateTime? created_dt { get; set; }

        public int? update_by { get; set; }

        public DateTime? update_dt { get; set; }

    }
    public class model_lesson_exercise
    {
        public int? id { get; set; }
        public string course_id { get; set; }
        public int? order { get; set; }
        public int? course_lesson_id { get; set; }
        public string question { get; set; }
        public string image { get; set; }
        public video video { get; set; }
        public int? is_answer_match { get; set; }
        public int? is_answer_choice { get; set; }
        public List<course_lesson_exercise_answer_choices> choices { get; set; }
        public List<course_lesson_exercise_answer_match> match { get; set; }
    }
}