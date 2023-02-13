using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DpimProject.Models.Data.DataModels
{
    [Table("student_lesson_learn")]
    public class student_lesson_learn //Create 10/23/2020 4:57:12 PM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(10)]
        public string lesson_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [MaxLength(255)]
        public string user_id { get; set; }

        [MaxLength(10)]
        public string learn_status { get; set; }

        public TimeSpan? timeline { get; set; }

    }
}