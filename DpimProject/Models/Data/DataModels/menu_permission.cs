using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("menu_permission")]
    public class menu_permission //Create 2/25/2021 4:18:46 AM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(10)]
        public string menu_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [MaxLength(30)]
        public string user_id { get; set; }

        [MaxLength(1)]
        public string status { get; set; }

    }
}