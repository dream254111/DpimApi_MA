using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DpimProject.Models.Data.DataModels
{
    [Table("user_online")]
    public class user_online //Create 10/16/2020 4:18:46 PM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(255)]
        public string session_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [MaxLength(255)]
        public string user_id { get; set; }

        [Key]
        [Column(Order = 2)]
        [MaxLength(20)]
        public string ip_address { get; set; }

        public DateTime? session_active { get; set; }

        public DateTime? session_start { get; set; }

        public DateTime? session_expired { get; set; }

    }
}