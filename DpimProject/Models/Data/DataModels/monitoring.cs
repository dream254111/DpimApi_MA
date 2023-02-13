using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DpimProject.Models.Data.DataModels
{
    [Table("monitoring")]
    public class monitoring //Create 10/23/2020 4:52:43 PM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(10)]
        public string monitoring_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [MaxLength(255)]
        public string user_id { get; set; }

        public string description { get; set; }

        [MaxLength(10)]
        public string status { get; set; }

        [MaxLength(10)]
        public string priority_type { get; set; }

        public DateTime? add_dt { get; set; }

        [MaxLength(255)]
        public string add_by { get; set; }

        public DateTime? edit_dt { get; set; }

        [MaxLength(255)]
        public string edit_by { get; set; }

        public DateTime? del_dt { get; set; }

        [MaxLength(255)]
        public string del_by { get; set; }

    }
}