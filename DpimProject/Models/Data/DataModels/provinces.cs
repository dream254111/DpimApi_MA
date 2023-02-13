using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("provinces")]
    public class provinces //Create 11/21/2020 3:46:31 AM
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? id { get; set; }

        [MaxLength(255)]
        public string province_code { get; set; }

        [MaxLength(255)]
        public string province_name { get; set; }

        [MaxLength(255)]
        public string province_name_eng { get; set; }

        public int? geo_id { get; set; }

    }
}