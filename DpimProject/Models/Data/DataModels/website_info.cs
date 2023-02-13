using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("website_info")]
    public class website_info
    {
        public int id { get; set; }
        public int visit_count { get; set; }
    }
}