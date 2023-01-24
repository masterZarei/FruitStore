using FS.DataAccess;
using FS.Models.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;

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
            var data = _db.Users.Where(a => a.UserName == userName).FirstOrDefault();

            return data;

        }
        public User GetInfoById(string Input)
        {
            var data = _db.Users.Where(a => a.Id == Input).FirstOrDefault();

            return data;

        }
        public string GetRoleById(string Input)
        {
            string result = _userManager.GetRolesAsync(new IdentityUser() { Id = Input }).Result[0].ToString();

            return result;

        }
        public string GetRoleByUserName(string Input)
        {
            string result = _userManager.GetRolesAsync(new IdentityUser() { UserName = Input }).Result[0].ToString();

            return result;

        }
       
        public  int AuthorizeUser(string Username)
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
