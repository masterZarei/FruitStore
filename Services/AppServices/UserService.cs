using FS.DataAccess;
using FS.Models.Models;
using System.Linq;

namespace Services.AppServices
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _db;

        public UserService(ApplicationDbContext db)
        {
            _db = db;
        }

        public User GetByUsername(string userName)
        {
            return _db.Users.FirstOrDefault(a => a.UserName == userName);
        }

        public User GetById(string id)
        {
            return _db.Users.FirstOrDefault(a => a.Id == id);
        }

        public bool IsDisabled(string userName)
        {
            var user = GetByUsername(userName);
            return user != null && user.isDisabled;
        }
    }
}