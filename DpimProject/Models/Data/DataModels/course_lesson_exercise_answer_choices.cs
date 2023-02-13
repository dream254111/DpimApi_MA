using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("course_lesson_exercise_answer_choices")]
    public class course_lesson_exercise_answer_choices //Create 11/21/2020 3:44:38 AM
    {
        [Key]
        [Column(Order = 0)]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? id { get; set; }

        public int? course_lesson_exercise_id { get; set; }

        public int? course_lesson_id { get; set; }
        
        public string answer { get; set; }

        public int? correct { get; set; }

        public int? order { get; set; }

    }
}