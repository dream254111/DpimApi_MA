using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DpimProject.Models.Data.DataModels
{
    [Table("certificate_print")]
    public class certificate_print //Create 1/15/2021 8:40:24 PM
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? itemno { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? student_id { get; set; }

        [MaxLength(100)]
        public string certificate_type { get; set; }

        public DateTime? print_dt { get; set; }

        public DateTime? certificate_dt { get; set; }

        public int? print_by { get; set; }

        [MaxLength(255)]
        public string certificate_id { get; set; }

        [MaxLength(10)]
        public string course_id { get; set; }

    }
}