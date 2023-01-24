using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.Models.Models
{
    public class Slider
    {
        [Key]
        public int Id { get; set; }
        //
        public string Img { get; set; }
        //
        public string Caption { get; set; }
        //
        public string Link { get; set; }
    }
}
