using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("online_user")]
    public class online_user //Create 11/21/2020 4:09:49 PM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(255)]
        public string session_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? user_id { get; set; }

        [Key]
        [Column(Order = 2)]
        [MaxLength(20)]
        public string ip_address { get; set; }

        public DateTime? session_start { get; set; }

        public DateTime? session_active { get; set; }

        public DateTime? session_expired { get; set; }

    }
}