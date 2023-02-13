using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace DpimProject.Models.Data.DataModels
{
    [Table("admin_menu")]
    public class admin_menu //Create 2/25/2021 8:18:46 AM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(10)]
        public string menu_id { get; set; }

        [MaxLength(-1)]
        public string menu_name { get; set; }

        [MaxLength(1)]
        public string menu_type { get; set; }

        [MaxLength(10)]
        public string menu_header { get; set; }

    }
}