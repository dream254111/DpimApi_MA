using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DpimProject.Models.Data.DataModels
{
    [Table("student_course_exam")]
    public class student_course_exam //Create 10/23/2020 4:56:20 PM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(255)]
        public string user_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [MaxLength(10)]
        public string course_question_id { get; set; }

        public int? answer_id { get; set; }

        public string answer_desc { get; set; }

        public decimal? score { get; set; }

    }
}