using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DpimProject.Models.Data.DataModels
{
    [Table("certificate_type")]
    public class certificate_type //Create 11/21/2020 2:13:51 AM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(20)]
        public string certificate_type_id { get; set; }

        [MaxLength(255)]
        public string certificate_type_desc { get; set; }

    }
}