using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("public_new")]
    public class public_new //Create 11/21/2020 3:46:48 AM
    {
        [Key]
        [Column(Order = 0)]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? id { get; set; }

        [MaxLength(255)]
        public string name { get; set; }

        [MaxLength(500)]
        public string image { get; set; }

        public int? active { get; set; }

        [MaxLength(500)]
        public string description { get; set; }

        public int? is_deleted { get; set; }

        public int? created_by { get; set; }

        public DateTime? created_at { get; set; }

        public int? update_by { get; set; }

        public DateTime? update_at { get; set; }

    }

    public class model_public_new
    {
        public int? id { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public bool active { get; set; }
        public string description { get; set; }
    }
}