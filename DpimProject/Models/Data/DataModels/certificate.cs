using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DpimProject.Models.Data.DataModels
{
    [Table("certificate")]
    public class certificate //Create 1/10/2021 11:08:37 PM
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? id { get; set; }

        [MaxLength(510)]
        public string manager_name { get; set; }

        public string path { get; set; }

        public DateTime? create_dt { get; set; }

        public int? create_by { get; set; }

        public int? is_delete { get; set; }

    }
}