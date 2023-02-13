using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("file_upload")]
    public class file_upload //Create 1/2/2021 11:18:02 PM
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? itemno { get; set; }

        public string filepath { get; set; }

        [MaxLength(255)]
        public string main_id { get; set; }

        [MaxLength(255)]
        public string seconde_id { get; set; }

        [MaxLength(255)]
        public string third_id { get; set; }

    }
}