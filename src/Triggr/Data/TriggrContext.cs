using Microsoft.EntityFrameworkCore;

namespace Triggr.Data
{
    public class TriggrContext : DbContext
    {
        public TriggrContext(DbContextOptions<TriggrContext> options)
            : base(options)
        {
            
        }

        public DbSet<Repository> Repositories { get; set; }
    }
}