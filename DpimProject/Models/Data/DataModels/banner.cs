using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("banner")]
    public class banner //Create 11/21/2020 3:36:54 AM
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? id { get; set; }

        public int? order { get; set; }

        public int? active { get; set; }

        [MaxLength(500)]
        public string image_pc { get; set; }

        [MaxLength(500)]
        public string image_mobile { get; set; }

        [MaxLength(255)]
        public string link { get; set; }

        public int? is_deleted { get; set; }

        public int? created_by { get; set; }

        public DateTime? created_at { get; set; }

        public int? update_by { get; set; }

        public DateTime? update_at { get; set; }

    }

    public class model_banner
    {
        public int? id { get; set; }
        public int? order { get; set; }
        public bool? active { get; set; }
        public string image_pc { get; set; }
        public string image_mobile { get; set; }
        public string link { get; set; }
    }
}