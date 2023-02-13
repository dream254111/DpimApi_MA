using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace DpimProject.Models.Data.DataModels
{
    [Table("questionnaire_header")]
    public class questionnaire_header //Create 10/23/2020 4:55:25 PM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(10)]
        public string questionnaire_question_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [MaxLength(10)]
        public string questionnaire_id { get; set; }

        [MaxLength(10)]
        public string question_type { get; set; }

        [MaxLength(255)]
        public string question_desc { get; set; }

    }
}