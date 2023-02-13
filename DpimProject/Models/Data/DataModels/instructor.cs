using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("instructor")]
    public class instructor //Create 11/21/2020 3:45:37 AM
    {
        [Key]
        [Column(Order = 0)]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? id { get; set; }

        [MaxLength(255)]
        public string title_name { get; set; }  [MaxLength(255)]
        public string firstname { get; set; }

        [MaxLength(255)]
        public string lastname { get; set; }
        [MaxLength(255)]
        public string firstname_en { get; set; }

        [MaxLength(255)]
        public string lastname_en { get; set; }

        [MaxLength(500)]
        public string profile_image { get; set; }

        [MaxLength(255)]
        public string position { get; set; }

        [MaxLength(255)]
        public string work { get; set; }

        [MaxLength(255)]
        public string email { get; set; }

        [MaxLength(255)]
        public string phone { get; set; }

        [MaxLength(255)]
        public string facebook { get; set; }

        [MaxLength(255)]
        public string twitter { get; set; }

        [MaxLength(255)]
        public string instagram { get; set; }

        public string description { get; set; }

        public int? is_deleted { get; set; }

        public int? created_by { get; set; }

        public DateTime? created_at { get; set; }

        public int? update_by { get; set; }

        public DateTime? update_at { get; set; }

    }
}