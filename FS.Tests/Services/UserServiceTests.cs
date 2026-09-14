using FS.Models.Models;
using Services.AppServices;
using Xunit;

namespace FS.Tests.Services
{
    public class UserServiceTests
    {
        [Fact]
        public void GetByUsername_ReturnsMatchingUser()
        {
            using var db = TestDb.Create();
            db.Users.Add(new User { Id = "u1", UserName = "09120000000", Name = "Ali", LastName = "Ahmadi" });
            db.SaveChanges();

            var service = new UserService(db);
            var user = service.GetByUsername("09120000000");

            Assert.NotNull(user);
            Assert.Equal("Ali", user.Name);
        }

        [Fact]
        public void GetByUsername_ReturnsNullForUnknownUser()
        {
            using var db = TestDb.Create();
            var service = new UserService(db);
            Assert.Null(service.GetByUsername("nobody"));
        }

        [Fact]
        public void GetById_ReturnsUserWhenExists()
        {
            using var db = TestDb.Create();
            db.Users.Add(new User { Id = "u1", UserName = "x", Name = "تست", LastName = "کاربر" });
            db.SaveChanges();

            var service = new UserService(db);
            Assert.NotNull(service.GetById("u1"));
        }

        [Fact]
        public void IsDisabled_TrueForDisabledUser()
        {
            using var db = TestDb.Create();
            db.Users.Add(new User { Id = "u1", UserName = "x", Name = "تست", LastName = "کاربر", isDisabled = true });
            db.SaveChanges();

            var service = new UserService(db);
            Assert.True(service.IsDisabled("x"));
        }

        [Fact]
        public void IsDisabled_FalseForActiveUser()
        {
            using var db = TestDb.Create();
            db.Users.Add(new User { Id = "u1", UserName = "x", Name = "تست", LastName = "کاربر", isDisabled = false });
            db.SaveChanges();

            var service = new UserService(db);
            Assert.False(service.IsDisabled("x"));
        }

        [Fact]
        public void IsDisabled_FalseForMissingUser()
        {
            using var db = TestDb.Create();
            var service = new UserService(db);
            Assert.False(service.IsDisabled("ghost"));
        }
    }
}