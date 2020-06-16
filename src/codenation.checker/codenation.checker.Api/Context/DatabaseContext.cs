using codenation.checker.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace codenation.checker.Api.Context
{
    public class DatabaseContext : DbContext
    {
        public DbSet<CodenationUser> CodenationUsers { get; set; }

        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }
    }
}
