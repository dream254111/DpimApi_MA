using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("course_permission")]
    public class course_permission //Create 1/16/2021 7:56:28 PM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(10)]
        public string course_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? student_type_permission { get; set; }

    }
}