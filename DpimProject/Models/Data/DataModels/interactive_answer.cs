using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DpimProject.Models.Data.DataModels
{
    [Table("interactive_answer")]
    public class interactive_answer
    {
        [Key]
        [Column(Order = 0)]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? id { get; set; }

        public int? interactive_question_id { get; set; }

        public int? course_lesson_id { get; set; }

        public string name { get; set; }

        public int? order { get; set; }

        public int? correct { get; set; }
    }
    
    public class model_interactive_answer
    {
        public int? id { get; set; }

        public int? interactive_question_id { get; set; }

        public int? course_lesson_id { get; set; }

        public string answer { get; set; }

        public int? order { get; set; }

        public int? correct { get; set; }
    }
}