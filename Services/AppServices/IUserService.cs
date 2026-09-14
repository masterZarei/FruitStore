using FS.Models.Models;

namespace Services.AppServices
{
    public interface IUserService
    {
        User GetByUsername(string userName);
        User GetById(string id);
        bool IsDisabled(string userName);
    }
}