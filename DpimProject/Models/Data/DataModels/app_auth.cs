using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace DpimProject.Models.Data.DataModels
{
    [Table("app_auth")]
    public class app_auth //Create 11/27/2020 4:48:35 PM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(255)]
        public string session_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? user_id { get; set; }

        public DateTime? start_dt { get; set; }

        public DateTime? end_dt { get; set; }

        public DateTime? active_dt { get; set; }

        [MaxLength(255)]
        public string passcode { get; set; }

        [MaxLength(50)]
        public string ip_address { get; set; }

        [MaxLength(255)]
        public string device { get; set; }
    }
}