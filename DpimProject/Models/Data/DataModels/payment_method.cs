using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("payment_method")]
    public class payment_method //Create 11/21/2020 3:46:05 AM
    {
        [Key]
        [Column(Order = 0)]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? id { get; set; }

        [MaxLength(255)]
        public string bank_name { get; set; }

        [MaxLength(255)]
        public string line { get; set; }

        [MaxLength(255)]
        public string account_name { get; set; }

        [MaxLength(255)]
        public string account_no { get; set; }

        [MaxLength(255)]
        public string qr_code { get; set; }

        public int? is_deleted { get; set; }

        public int? created_by { get; set; }

        public DateTime? created_at { get; set; }

        public int? update_by { get; set; }

        public DateTime? update_at { get; set; }

    }
}