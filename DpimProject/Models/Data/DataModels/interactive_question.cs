using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DpimProject.Models.Data.DataModels
{
    [Table("interactive_question")]
    public class interactive_question //Create 1/15/2021 9:00:21 PM
    {
        [Key]
        [Column(Order = 0)]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }

        [MaxLength(255)]
        public string course_id { get; set; }

        public int? course_lesson_id { get; set; }

        public int? order { get; set; }
       
        public string name { get; set; }
        
        public string interactive_time { get; set; }

        public int? is_deleted { get; set; }

        public int? created_by { get; set; }

        public DateTime? created_at { get; set; }

        public int? update_by { get; set; }

        public DateTime? update_at { get; set; }

    }
    public class model_interactive_question
    {
        public int? id { get; set; }

        public string course_id { get; set; }

        public int? course_lesson_id { get; set; }

        public string question { get; set; }

        public int? order { get; set; }

        public string interactive_time { get; set; }
    }
}