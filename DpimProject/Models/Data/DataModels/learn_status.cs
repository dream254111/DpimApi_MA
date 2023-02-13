using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DpimProject.Models.Data.DataModels
{
    [Table("learn_status")]
    public class learn_status //Create 10/23/2020 4:51:01 PM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(10)]
        public string learn_status_id { get; set; }

        [MaxLength(255)]
        public string learn_status_desc { get; set; }

    }
}