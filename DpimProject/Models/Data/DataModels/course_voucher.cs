using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("course_voucher")]
    public class course_voucher //Create 1/16/2021 8:54:41 AM
    {
        [Key]
        [Column(Order = 0)]
        public int id { get; set; }

        [MaxLength(10)]
        public string voucher_id { get; set; }
        
        [MaxLength(10)]
        public string course_id { get; set; }

        public DateTime? start_dt { get; set; }

        public DateTime? end_dt { get; set; }

        public int? is_delete { get; set; }

        public int? created_by { get; set; }

        public DateTime? created_at { get; set; }

        public int? update_by { get; set; }

        public DateTime? update_dt { get; set; }

        public int? status { get; set; }
    }

    public class model_course_voucher
    {
        public int? id { get; set; }
        public string voucher_id { get; set; }
        public string course_id { get; set; }
        public DateTime? start_dt { get; set; }
        public DateTime? end_dt { get; set; }
    }
}