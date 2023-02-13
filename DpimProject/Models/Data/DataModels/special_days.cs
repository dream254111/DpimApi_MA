using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DpimProject.Models.Data.DataModels
{
    [Table("special_days")]
    public class special_days
    {
        [Key]
        [Column(Order = 0)]
        public int? id { get; set; }

        public string cover { get; set; }

        public DateTime? start_date { get; set; }

        public DateTime? end_date { get; set; }

        public int? is_deleted { get; set; }

        public int? created_by { get; set; }

        public DateTime? created_at { get; set; }

        public int? update_by { get; set; }

        public DateTime? update_at { get; set; }
    }
}