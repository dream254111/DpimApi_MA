using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace DpimProject.Models.Data.DataModels
{
    [Table("course")]
    public class course //Create 1/15/2021 8:27:32 PM
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(10)]
        public string id { get; set; }

        [Key]
        [Column(Order = 1)]
        [MaxLength(2)]
        public string course_category_id { get; set; }

        [Key]
        [Column(Order = 2)]
        [MaxLength(2)]
        public string department_id { get; set; }

        [MaxLength(510)]
        public string name { get; set; }

        public string cover_pic { get; set; }

        public string info_cover { get; set; }

        public int? is_learning_online { get; set; }

        public int? is_has_cost { get; set; }

        public decimal? cost { get; set; }

        [MaxLength(-1)]
        public string overview_course { get; set; }

        [MaxLength(-1)]
        public string objective_course { get; set; }

        public int? print_count { get; set; }

        [MaxLength(-1)]
        public string benefits { get; set; }

        public int? batch { get; set; }

        public int? hasCertificate { get; set; }

        public int? isAlwaysRegister { get; set; }

        public DateTime? register_start_date { get; set; }

        public DateTime? register_end_date { get; set; }

        public int? is_always_learning { get; set; }

        public DateTime? learning_startdate { get; set; }

        public DateTime? learning_enddate { get; set; }

        public string video_sample { get; set; }

        public string p_480 { get; set; }

        public string p_720 { get; set; }

        public string p_1080 { get; set; }

        public string p_original { get; set; }

        public string cover_video { get; set; }

        [MaxLength(510)]
        public string contact_name { get; set; }

        [MaxLength(510)]
        public string contact_phone { get; set; }

        [MaxLength(510)]
        public string contact_email { get; set; }

        public int? is_deleted { get; set; }

        public int? created_by { get; set; }

        public DateTime? created_dt { get; set; }

        public int? update_by { get; set; }

        public DateTime? update_dt { get; set; }

        public int? passed_percent { get; set; }

        public string sub_name { get; set; }

    }
    public class model_course
    {
        public string id { get; set; }
        public int? batch { get; set; }
        public string benefits { get; set; }
        public string categoryId { get; set; }
        public string departmentId { get; set; }
        public string contactEmail { get; set; }
        public string contactName { get; set; }
        public string contactPhone { get; set; }
        public decimal? cost { get; set; }
        public string cover { get; set; }
        public string info_cover { get; set; }
        public bool? hasCost { get; set; }
        public bool? hasCertificate { get; set; }
        public bool? isAlwaysRegister { get; set; }
        public DateTime? register_start_date { get; set; }
        public DateTime? register_end_date { get; set; }
        public bool? isAlwaysLearning { get; set; }
        public DateTime? learning_enddate { get; set; }
        public DateTime? learning_startdate { get; set; }
        public string name { get; set; }
        public string sub_name { get; set; }
        public string objective { get; set; }
        public string overview { get; set; }
        public int? type { get; set; }
        public video video { get; set; }
        public int? passed_percent { get; set; }
        public List<int> content_permission { get; set; }
    }
}