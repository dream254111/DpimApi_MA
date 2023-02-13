using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("group_department")]
    public class group_department //Create 2/25/2021 11:10:37 AM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(10)]
        public string depart_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [MaxLength(10)]
        public string user_id { get; set; }

        [MaxLength(1)]
        public string status { get; set; }

    }
}