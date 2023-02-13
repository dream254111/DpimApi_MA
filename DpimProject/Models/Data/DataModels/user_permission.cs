using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DpimProject.Models.Data.DataModels
{
    [Table("user_permisson")]
    public class user_permission //Create 10/16/2020 4:28:57 PM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(255)]
        public string user_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [MaxLength(255)]
        public string permission_id { get; set; }

    }
}