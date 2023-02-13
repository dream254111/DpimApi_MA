using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("email_type")]
    public class email_type //Create 11/24/2020 9:42:28 PM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(20)]
        public string email_type_id { get; set; }

        [MaxLength(255)]
        public string email_type_desc { get; set; }

        [MaxLength(510)]
        public string subject { get; set; }

        public string path { get; set; }

    }
}