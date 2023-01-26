using System.ComponentModel.DataAnnotations;

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
