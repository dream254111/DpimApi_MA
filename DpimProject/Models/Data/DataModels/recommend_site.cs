using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("recommend_site")]
    public class recommend_site //Create 11/21/2020 3:47:02 AM
    {
        [Key]
        [Column(Order = 0)]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? id { get; set; }

        [MaxLength(255)]
        public string name1 { get; set; }

        [MaxLength(255)]
        public string link1 { get; set; }

        [MaxLength(255)]
        public string name2 { get; set; }

        [MaxLength(255)]
        public string link2 { get; set; }

        [MaxLength(255)]
        public string name3 { get; set; }

        [MaxLength(255)]
        public string link3 { get; set; }

        [MaxLength(255)]
        public string name4 { get; set; }

        [MaxLength(255)]
        public string link4 { get; set; }

        public string cover1 { get; set; }

        public string cover2 { get; set; }

        public string cover3 { get; set; }

        public string cover4 { get; set; }

    }
}