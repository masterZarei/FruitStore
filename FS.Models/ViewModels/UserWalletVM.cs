using FS.Models.Models;
using System.Collections.Generic;

namespace FS.Models.ViewModels
{
    public class UserWalletVM
    {
        public User ApplicationUser { get; set; }
        public List<WalletHistory> WalletHistory { get; set; }
    }
}
