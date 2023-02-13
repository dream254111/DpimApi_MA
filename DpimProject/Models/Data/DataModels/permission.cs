using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DpimProject.Models.Data.DataModels
{
    [Table("permission")]
    public class permission //Create 10/16/2020 4:18:16 PM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(10)]
        public string permission_id { get; set; }

        [MaxLength(255)]
        public string permission_desc { get; set; }

    }
}