using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DpimProject.Models.Data.DataModels
{
    [Table("student_questionnaire")]
    public class student_questionnaire //Create 10/23/2020 4:58:42 PM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(255)]
        public string user_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [MaxLength(10)]
        public string questionnaire_question_id { get; set; }

        public int? answer_id { get; set; }

        public string answer_desc { get; set; }

    }
}