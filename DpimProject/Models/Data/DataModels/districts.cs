using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("districts")]
    public class districts //Create 11/21/2020 3:45:07 AM
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? id { get; set; }

        public int? province_id { get; set; }

        [MaxLength(255)]
        public string districts_code { get; set; }

        [MaxLength(255)]
        public string districts_name { get; set; }

        [MaxLength(255)]
        public string districts_name_eng { get; set; }

        public int? geo_id { get; set; }

    }
}