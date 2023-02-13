using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("zipcode")]
    public class zipcode
    {
        public int id { get; set; }
        public string districts_code { get; set; }
        public string zipcode_name { get; set; }
    }
}