using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace DpimProject.Models.Data.DataModels
{
    [Table("course_exam_header")]
    public class course_exam_header //Create 10/23/2020 4:48:44 PM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(10)]
        public string exam_question_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [MaxLength(10)]
        public string course_exam_id { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? answer_id { get; set; }

        [MaxLength(10)]
        public string question_type { get; set; }

        [MaxLength(255)]
        public string question_desc { get; set; }

        public decimal? score { get; set; }

    }
}