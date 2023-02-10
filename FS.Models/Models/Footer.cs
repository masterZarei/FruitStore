using System.ComponentModel.DataAnnotations;

namespace FS.Models.Models
{
    public class Footer
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Description { get; set; }
        public string TrustSymbol { get; set; }
        public string TrustSymbol2 { get; set; }

    }
}
