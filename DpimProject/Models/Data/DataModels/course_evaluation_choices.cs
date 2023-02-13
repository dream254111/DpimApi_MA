using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("course_evaluation_choices")]
    public class course_evaluation_choices //Create 11/21/2020 3:41:25 AM
    {
        [Key]
        [Column(Order = 0)]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? id { get; set; }

        public int? course_evaluation_id { get; set; }
        
        public string choice { get; set; }

        public decimal? score { get; set; }

        public int? order { get; set; }

    }
}