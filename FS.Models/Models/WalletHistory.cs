using FS.Models.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FS.Models.Models
{
    public class WalletHistory : BaseClass
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public int TrackingCode { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal NewWalletAmount { get; set; }
        public bool State { get; set; }
    }
}
