using FS.Models.BaseEntities;

namespace FS.Models.Models
{
    public class Discount: BaseClass
    {
        public int Id { get; set; }
        public double Percent { get; set; }
        public string Occasion { get; set; }
    }
}
