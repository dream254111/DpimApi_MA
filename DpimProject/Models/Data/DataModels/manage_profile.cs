using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("manage_profile")]
    public class manage_profile //Create 11/21/2020 3:45:50 AM
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? id { get; set; }

        public int? is_edit_personal_info { get; set; }

        public int? is_edit_address { get; set; }

        public int? is_edit_email { get; set; }

        public int? is_edit_phone { get; set; }

        public int? is_edit_educational { get; set; }

        public int? is_edit_career { get; set; }

        public int? is_edit_know_channel { get; set; }

        public int? is_edit_business { get; set; }

        public int? is_deleted { get; set; }

        public int? created_by { get; set; }

        public DateTime? created_at { get; set; }

        public int? update_by { get; set; }

        public DateTime? update_at { get; set; }

    }
}