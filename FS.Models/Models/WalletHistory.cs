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

        public double TrackingCode { get; set; }
        public double TransactionAmount { get; set; }
        public double NewWalletAmount { get; set; }
        public string State { get; set; }
    }
}
