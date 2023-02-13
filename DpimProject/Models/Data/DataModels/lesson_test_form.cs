using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DpimProject.Models.Data.DataModels
{
    [Table("lesson_test_form")]
    public class lesson_test_form //Create 10/23/2020 4:51:57 PM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(10)]
        public string lesson_test_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [MaxLength(10)]
        public string lesson_id { get; set; }

        [MaxLength(10)]
        public string status { get; set; }

        public decimal? pass_percent { get; set; }

        [MaxLength(255)]
        public string add_by { get; set; }

        public DateTime? add_dt { get; set; }

        [MaxLength(255)]
        public string edit_by { get; set; }

        public DateTime? edit_dt { get; set; }

        [MaxLength(255)]
        public string del_by { get; set; }

        public DateTime? del_dt { get; set; }

    }
}