using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DpimProject.Models.Data.DataModels
{
    [Table("faq_group")]
    public class faq_group //Create 10/23/2020 4:50:18 PM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(10)]
        public string faq_group_id { get; set; }

        [MaxLength(255)]
        public string faq_group_desc { get; set; }

    }
}