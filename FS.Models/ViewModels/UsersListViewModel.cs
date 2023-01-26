using FS.Models.Models;
using FS.Models.Paging;
using System.Collections.Generic;

namespace FS.Models.ViewModels
{
    public class UsersListViewModel
    {
        public List<User> ApplicationUserList { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
