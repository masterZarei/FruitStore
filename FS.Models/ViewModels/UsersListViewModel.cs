using FS.Models.Models;
using FS.Models.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS.Models.ViewModels
{
    public class UsersListViewModel
    {
        public List<User> ApplicationUserList { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
