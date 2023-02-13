using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("report_problem")]
    public class report_problem //Create 11/21/2020 3:47:12 AM
    {
        [Key]
        [Column(Order = 0)]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? id { get; set; }

        public int? problem_of_use_id { get; set; }

        [MaxLength(255)]
        public string firstname { get; set; }

        [MaxLength(255)]
        public string lastname { get; set; }
        
        public string description { get; set; }

        public string response { get; set; }

        [MaxLength(255)]
        public string phone { get; set; }

        [MaxLength(255)]
        public string email { get; set; }

        public int? status { get; set; }

        public int? is_deleted { get; set; }

        public int? created_by { get; set; }

        public DateTime? created_at { get; set; }

        public int? update_by { get; set; }

        public DateTime? update_at { get; set; }
    }

    public class model_report_problem
    {
        public int? problem_of_use_id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string description { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
    }
}