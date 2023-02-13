using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("tutorial_detail")]
    public class tutorial_detail //Create 11/22/2020 8:49:43 PM
    {
        [Key]
        [Column(Order = 0)]
        public int? detail_id { get; set; }
        
        public int? tutorial_id { get; set; }

        public int? order { get; set; }

        public string img_path { get; set; }

        public string title { get; set; }

    }
}