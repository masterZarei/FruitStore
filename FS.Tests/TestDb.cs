using FS.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;

namespace FS.Tests
{
    internal static class TestDb
    {
        public static ApplicationDbContext Create()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }
    }
}