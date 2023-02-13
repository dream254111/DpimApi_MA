using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DpimProject.Models.Data.DataModels
{
    [Table("certificate_info")]
    public class certificate_info //Create 1/16/2021 3:32:40 PM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(255)]
        public string certificate_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? student_id { get; set; }

        [Key]
        [Column(Order = 2)]
        [MaxLength(10)]
        public string course_id { get; set; }

        public DateTime? certificate_dt { get; set; }

        public string path { get; set; }

        public int? created_by { get; set; }

        public DateTime? created_dt { get; set; }

        public int? update_by { get; set; }

        public DateTime? update_dt { get; set; }

        [MaxLength(1)]
        public string cert_status { get; set; }

    }
}