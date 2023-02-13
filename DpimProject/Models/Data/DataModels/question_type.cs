using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace DpimProject.Models.Data.DataModels
{
    [Table("question_type")]
    public class question_type //Create 10/23/2020 4:54:09 PM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(10)]
        public string question_type_id { get; set; }

        [MaxLength(255)]
        public string question_desc { get; set; }

    }
}