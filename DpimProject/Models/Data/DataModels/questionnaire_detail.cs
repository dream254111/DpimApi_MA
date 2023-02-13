using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DpimProject.Models.Data.DataModels
{
    [Table("questionnaire_detail")]
    public class questionnaire_detail //Create 10/23/2020 4:54:40 PM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(10)]
        public string questionnaire_question_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [MaxLength(10)]
        public string questionnaire_id { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? answer_id { get; set; }

        public string answer_desc { get; set; }

    }
}