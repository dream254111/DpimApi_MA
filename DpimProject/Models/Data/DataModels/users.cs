using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DpimProject.Models.Data.DataModels
{
    [Table("users")]
    public class users //Create 10/23/2020 5:22:54 PM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(255)]
        public string user_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [MaxLength(13)]
        public string nation_id { get; set; }

        [MaxLength(255)]
        public string user_name { get; set; }

        [MaxLength(255)]
        public string user_pass { get; set; }

        [MaxLength(255)]
        public string title_name { get; set; }

        [MaxLength(255)]
        public string first_name { get; set; }

        [MaxLength(255)]
        public string last_name { get; set; }

        [MaxLength(255)]
        public string organization { get; set; }

        [MaxLength(10)]
        public string sex { get; set; }

        [MaxLength(10)]
        public string status_type_id { get; set; }

        [MaxLength(255)]
        public string email { get; set; }

        public string pic_profile { get; set; }

        [MaxLength(255)]
        public string add_by { get; set; }

        public DateTime? add_dt { get; set; }

        [MaxLength(255)]
        public string edit_by { get; set; }

        public DateTime? edit_dt { get; set; }

        [MaxLength(255)]
        public string del_by { get; set; }

        public DateTime? del_dt { get; set; }

        public DateTime? date_of_birth { get; set; }

        [MaxLength(10)]
        public string group_id { get; set; }

    }
}