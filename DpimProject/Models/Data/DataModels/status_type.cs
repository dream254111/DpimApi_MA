using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DpimProject.Models.Data.DataModels
{
    [Table("status_type")]
    public class status_type //Create 10/16/2020 4:18:58 PM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(10)]
        public string status_type_id { get; set; }

        [MaxLength(255)]
        public string status_type_desc { get; set; }

    }
}