using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace Utilities.Convertors
{
    public class GetUserInfo
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;


        public GetUserInfo(ApplicationDbContext db, UserManager<IdentityUser> userManager = null)
        {
            _db = db;
            _userManager = userManager;
        }

        public User GetInfoByUsername(string userName)
        {
            var data = _db.Users.FirstOrDefault(a => a.UserName == userName);

            return data;

        }
        public User GetInfoById(string Input)
        {
            var data = _db.Users.FirstOrDefault(a => a.Id == Input);

            return data;

        }
        public async Task<string> GetRoleByIdAsync(string Id)
        {
            if (_userManager == null)
                return null;

            var roles = await _userManager.GetRolesAsync(new User { Id = Id });

            return roles.FirstOrDefault();

        }
        public async Task<string> GetRoleByUserNameAsync(string userName)
        {
            if (_userManager == null)
                return null;

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);

            return roles.FirstOrDefault();

        }
       
        public int AuthorizeUser(string Username)
        {
            User ap = GetInfoByUsername(Username);
            if (ap == null)
                return -1;

           else if (ap.isDisabled)
                return 1;

            else
                return 0;

        }

    }
}
