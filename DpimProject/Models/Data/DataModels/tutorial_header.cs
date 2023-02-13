using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DpimProject.Models.Data.DataModels
{
    [Table("tutorial_header")]
    public class tutorial_header //Create 11/22/2020 8:48:25 PM
    {
        [Key]
        [Column(Order = 0)]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? tutorial_id { get; set; }

        public int? order { get; set; }

        public string tutorial_text { get; set; }

        public string image { get; set; }

        public string link { get; set; }
    }
 
    public class model_tutorial
    {
        public tutorial_header header { get; set; }
        public List<tutorial_detail> detail { get; set; }
    }
}