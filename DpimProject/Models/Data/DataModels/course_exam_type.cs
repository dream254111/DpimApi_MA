using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DpimProject.Models.Data.DataModels
{
    [Table("course_exam_type")]
    public class course_exam_type //Create 12/2/2020 8:31:41 PM
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? type_exam_id { get; set; }

        [MaxLength(255)]
        public string type_exam_desc { get; set; }

    }
}