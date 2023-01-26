using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FS.Models.Models
{
    public class FactorDetail
    {
        [Key]
        public int DetailId { get; set; }
        //
        [ForeignKey("FactorId")]
        public virtual Factor Factor { get; set; }
        public int FactorId { get; set; }
        //
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        [Required]
        public int ProductId { get; set; }
        //
        [Required]
        public double Price { get; set; }
        //
        [Required]
        public int Count { get; set; }
    }
}
