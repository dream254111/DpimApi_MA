using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("course_evaluation_result")]
    public class course_evaluation_result //Create 1/15/2021 8:30:28 PM
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? course_evaluation_id { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? student_id { get; set; }

        [Key]
        [Column(Order = 3)]
        [MaxLength(10)]
        public string course_id { get; set; }

        public int? course_evaluation_choices_id { get; set; }

        [MaxLength(-1)]
        public string answer { get; set; }

        public int? is_deleted { get; set; }

        public int? created_by { get; set; }

        public DateTime? created_dt { get; set; }

        public int? update_by { get; set; }

        public DateTime? update_dt { get; set; }

    }
    public class model_evaluation_result
    {
        public int course_evaluation_id { get; set; }
        public int course_evaluation_choices_id { get; set; }
        public string answer { get; set; }
    }
}