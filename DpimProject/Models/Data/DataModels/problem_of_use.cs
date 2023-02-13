using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("problem_of_use")]
    public class problem_of_use
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}