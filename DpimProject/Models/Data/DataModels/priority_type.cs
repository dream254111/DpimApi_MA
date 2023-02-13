using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace DpimProject.Models.Data.DataModels
{
    [Table("priority_type")]
    public class priority_type //Create 10/23/2020 4:53:42 PM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(10)]
        public string priority_type_id { get; set; }

        [MaxLength(255)]
        public string priority_type_desc { get; set; }

    }
}