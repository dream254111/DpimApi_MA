using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DpimProject.Models.Data.DataModels
{
    [Table("monitoring_status")]
    public class monitoring_status //Create 10/23/2020 4:53:20 PM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(10)]
        public string monitoring_status_id { get; set; }

        [MaxLength(255)]
        public string monitoring_desc { get; set; }

    }
}