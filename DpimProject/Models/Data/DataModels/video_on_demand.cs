using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("video_on_demand")]
    public class video_on_demand //Create 1/15/2021 8:31:22 PM
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? id { get; set; }

        [MaxLength(2)]
        public string course_category_id { get; set; }

        [MaxLength(510)]
        public string name { get; set; }

        [MaxLength(-1)]
        public string video { get; set; }

        public int? count_view { get; set; }

        [MaxLength(-1)]
        public string description { get; set; }

        [MaxLength(1000)]
        public string producer_name { get; set; }

        [MaxLength(1000)]
        public string phone { get; set; }

        [MaxLength(1000)]
        public string email { get; set; }

        [MaxLength(-1)]
        public string attachment { get; set; }
        
        public string video_sample { get; set; }

        public string p_480 { get; set; }

        public string p_720 { get; set; }

        public string p_1080 { get; set; }

        public string p_original { get; set; }

        public string cover_video { get; set; }

        public string cover_thumbnail { get; set; }

        public int? is_deleted { get; set; }

        public int? created_by { get; set; }

        public DateTime? created_dt { get; set; }

        public int? update_by { get; set; }

        public DateTime? update_dt { get; set; }

        public int? type { get; set; }

        public string youtube { get; set; }

    }

    public class model_video_on_demand
    {
        public int? id { get; set; }
        public string course_category_id { get; set; }
        public string name { get; set; }
        public string link_video { get; set; }
        public int? count_view { get; set; }
        public string description { get; set; }
        public string producer_name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string attachment { get; set; }
        public string cover_thumbnail { get; set; }
        public video video { get; set; }
        public List<int> content_permission { get; set; }
        public int? type { get; set; }
        public string youtube { get; set; }
    }
}