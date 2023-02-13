using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace DpimProject.Models.Data.DataModels
{
    [Table("department")]
    public class department //Create 1/15/2021 4:09:27 PM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(10)]
        public string id { get; set; }

        [MaxLength(510)]
        public string department_name { get; set; }

        public int? create_by { get; set; }

        public DateTime? create_dt { get; set; }

        public int? update_by { get; set; }

        public DateTime? update_dt { get; set; }

        public string department_name_short { get; set; }
    }
}