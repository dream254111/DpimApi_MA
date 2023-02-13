using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DpimProject.Models.Data.DataModels
{
    [Table("student_group")]
    public class student_group //Create 10/23/2020 4:56:44 PM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(10)]
        public string student_group_id { get; set; }

        [MaxLength(255)]
        public string student_group_desc { get; set; }

    }
}