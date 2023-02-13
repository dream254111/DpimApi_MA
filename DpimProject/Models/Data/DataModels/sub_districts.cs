using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("sub_districts")]
    public class sub_districts
    {
        public int id { get; set; }
        public int districts_id { get; set; }
        public int province_id { get; set; }
        public string sub_districts_code { get; set; }
        public string sub_districts_name { get; set; }
        public string sub_districts_name_eng { get; set; }
        public int geo_id { get; set; }
    }
}