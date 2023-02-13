using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("user")]
    public class user //Create 11/30/2020 1:16:39 PM
    {
        [Key]
        [Column(Order = 0)]
        public int? id { get; set; }

        [MaxLength(255)]
        public string username { get; set; }

        [Key]
        [Column(Order = 2)]
        [MaxLength(255)]
        public string email { get; set; }

        public int? role_id { get; set; }

        [MaxLength(255)]
        public string password { get; set; }

        public int? is_deleted { get; set; }

        public int? created_by { get; set; }

        public DateTime? created_at { get; set; }

        public int? update_by { get; set; }

        public DateTime? update_at { get; set; }

    }
  }