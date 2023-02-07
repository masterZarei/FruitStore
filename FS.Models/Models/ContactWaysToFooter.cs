using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FS.Models.Models
{
    public class ContactWaysToFooter
    {
        [Key]
        public int Id { get; set; }
 
        public int ContactWaysId { get; set; }
        [ForeignKey("ContactWaysId")]
        public virtual ContactWays ContactWays { get; set; }
    }
}
