using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("course_category")]
    public class course_category //Create 1/15/2021 8:30:02 PM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(2)]
        public string id { get; set; }

        [MaxLength(510)]
        public string name { get; set; }

        [MaxLength(510)]
        public string color { get; set; }

    }
}