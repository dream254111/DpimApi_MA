using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DpimProject.Models.Data.DataModels
{
    [Table("catagory")]
    public class catagory //Create 10/23/2020 4:44:32 PM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(10)]
        public string cat_id { get; set; }

        [MaxLength(255)]
        public string cat_name { get; set; }

    }
}