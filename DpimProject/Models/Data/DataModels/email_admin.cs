using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("email_admin")]
    public class email_admin //Create 11/24/2020 4:17:33 PM
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? email_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [MaxLength(255)]
        public string email { get; set; }

        [MaxLength(255)]
        public string password { get; set; }

        [MaxLength(255)]
        public string smtp { get; set; }

        [MaxLength(510)]
        public string admin_name { get; set; }

        [MaxLength(510)]
        public string email_type { get; set; }

    }
}