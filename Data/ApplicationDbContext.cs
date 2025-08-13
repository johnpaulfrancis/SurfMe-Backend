using Microsoft.EntityFrameworkCore;

namespace SurfMe.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Models.TableSchema.UserTableModel> Tbl_Users { get; set; }
        public DbSet<Models.TableSchema.APILoggerTableModel>Tbl_APILogger { get; set; }
    }
}
