using FS.Models.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace FS.Models.Models
{
    public class Logo : BaseClass
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public bool DeliverAtTheSameDate { get; set; }
    }
}
