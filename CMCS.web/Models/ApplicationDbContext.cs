using Microsoft.EntityFrameworkCore;

namespace CMCS.Web.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions opts) : base(opts) { }

        public DbSet<Claim> Claims { get; set; }
        public DbSet<UploadFile> UploadFiles { get; set; }
    }
}

