using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("course_exam")]
    public class course_exam //Create 1/15/2021 8:29:07 PM
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? id { get; set; }

        [Key]
        [Column(Order = 1)]
        [MaxLength(10)]
        public string course_id { get; set; }

        public int? order { get; set; }

        [MaxLength(-1)]
        public string question { get; set; }

        public decimal? percent_pass { get; set; }

        public int? score { get; set; }

        public int? answer { get; set; }

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

        [MaxLength(1)]
        public string question_random { get; set; }

        public int? is_deleted { get; set; }

        public int? created_by { get; set; }

        public DateTime? created_dt { get; set; }

        public int? update_by { get; set; }

        public DateTime? update_dt { get; set; }

    }
    public class model_exam
    {
        public int? id { get; set; }
        public string course_id { get; set; }
        public int? order { get; set; }
        public string question { get; set; }
        public string image { get; set; }
        public dynamic video { get; set; }
        public decimal? percent_pass { get; set; }
        public int? score { get; set; }
        public int? answer { get; set; }
    }

    public class model_exam_platform
    {
        public int? id { get; set; }
        public string course_id { get; set; }
        public int? order { get; set; }
        public string question { get; set; }
        public string image { get; set; }
        public video video { get; set; }
        public List<model_answer_exam_platform> list_answer { get; set; }
    }

    public class model_answer_exam_platform
    {
        public int? id { get; set; }
        public int? course_exam_id { get; set; }
        public string course_id { get; set; }
        public int? order { get; set; }
        public string answer { get; set; }
    }
}