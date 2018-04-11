using Microsoft.EntityFrameworkCore;

namespace Triggr.Data
{
    public class TriggrContext : DbContext
    {
        public TriggrContext()
        {
            
        }
        public TriggrContext(DbContextOptions<TriggrContext> options)
            : base(options)
        {
            
        }

        public virtual DbSet<Repository> Repositories { get; set; }
    }
}