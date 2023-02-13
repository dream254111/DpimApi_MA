using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DpimProject.Models.Data.DataModels
{
    [Table("student_lesson_test")]
    public class student_lesson_test //Create 10/23/2020 4:58:21 PM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(255)]
        public string user_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [MaxLength(10)]
        public string test_question_id { get; set; }

        public int? answer_id { get; set; }

        public string answer_desc { get; set; }

        public decimal? score { get; set; }

    }
}