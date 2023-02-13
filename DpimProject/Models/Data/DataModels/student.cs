using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("student")]
    public class student //Create 12/24/2020 9:12:39 PM
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? user_id { get; set; }

        public int? id { get; set; }

        public int? course_id { get; set; }

        public int? student_id { get; set; }

        [MaxLength(510)]
        public string username { get; set; }

        [MaxLength(510)]
        public string password { get; set; }

        public int? career_id { get; set; }

        [MaxLength(500)]
        public string career_name { get; set; }

        public int? business_type_id { get; set; }

        public int? student_account_type_id { get; set; }

        [MaxLength(510)]
        public string profile_image { get; set; }

        [MaxLength(510)]
        public string firstname { get; set; }

        [MaxLength(510)]
        public string lastname { get; set; }

        [MaxLength(510)]
        public string firstname_en { get; set; }

        [MaxLength(510)]
        public string lastname_en { get; set; }

        [MaxLength(255)]
        public string title_name { get; set; }

        [MaxLength(255)]
        public string title_name_en { get; set; }

        [MaxLength(510)]
        public string id_card { get; set; }

        public int? gender { get; set; }

        public DateTime? birthday { get; set; }

        [MaxLength(1000)]
        public string address { get; set; }

        public int? sub_district_id { get; set; }

        public int? district_id { get; set; }

        public int? province_id { get; set; }

        [MaxLength(510)]
        public string zipcode { get; set; }

        [MaxLength(510)]
        public string email { get; set; }

        [MaxLength(510)]
        public string phone { get; set; }

        public int? educational_id { get; set; }

        [MaxLength(510)]
        public string position { get; set; }

        public int? know_channel { get; set; }
        
        public string know_channel_name { get; set; }

        [MaxLength(1000)]
        public string business_name { get; set; }

        public int? business_province_id { get; set; }

        public DateTime? business_register { get; set; }

        [MaxLength(510)]
        public string business_no { get; set; }

        public int? is_internal_student { get; set; }

        [MaxLength(1000)]
        public string front_id_card { get; set; }

        [MaxLength(1000)]
        public string back_id_card { get; set; }

        [MaxLength(1000)]
        public string straight_face_image { get; set; }

        [MaxLength(1000)]
        public string business_attachment { get; set; }
        [MaxLength(255)]
        public string id_dpim { get; set; }

        public int? is_deleted { get; set; }

        public int? created_by { get; set; }

        public DateTime? created_at { get; set; }

        public int? update_by { get; set; }

        public DateTime? update_at { get; set; }

        public string pdpa_accept { get; set; }

        public DateTime? pdpa_stamp { get; set; }

    }

    public class model_register
    {
        public string email { get; set; }
        public string id_dpim { get; set; }
        public string password { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string firstname_en { get; set; }
        public string lastname_en { get; set; }
        public string id_card { get; set; }
        public int gender { get; set; }
        public DateTime birthday { get; set; }
        public int province_id { get; set; }
        public int district_id { get; set; }
        public int sub_district_id { get; set; }
        public string zipcode { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public int educational_id { get; set; }
        public int career_id { get; set; }
        public string career_name { get; set; }
        public List<int> know_channel { get; set; }
        public string know_channel_name { get; set; }
    }

    public class model_update_profile
    {
        public string profile_image { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string firstname_en { get; set; }
        public string lastname_en { get; set; }
        public string id_card { get; set; }
        public int? gender { get; set; }
        public DateTime? birthday { get; set; }
        public int? province_id { get; set; }
        public int? district_id { get; set; }
        public int? sub_district_id { get; set; }
        public string zipcode { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public int? educational_id { get; set; }
        public int? career_id { get; set; }
        public string career_name { get; set; }
        public List<int> know_channel { get; set; }
        public string business_name { get; set; }
        public int? business_type_id { get; set; }
        public int? business_province_id { get; set; }
        public DateTime? business_register { get; set; }
        public string business_no { get; set; }
        public string front_id_card { get; set; }
        public string back_id_card { get; set; }
        public string straight_face_image { get; set; }
        public string business_attachment { get; set; }
        public string know_channel_name { get; set; }
    }   
}