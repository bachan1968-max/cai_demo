using Microsoft.EntityFrameworkCore;

namespace cai.Service.Database
{
    public class CaiDbContext : DbContext
    {
        public DbSet<PriceList> PriceLists { get; set; }
        public DbSet<PriceListRow> PriceListsRow { get; set; }

        public CaiDbContext(DbContextOptions<CaiDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<PriceListRow>()
                .HasOne<PriceList>()
                .WithMany()
                .HasForeignKey(p => p.Id);
            base.OnModelCreating(modelBuilder);
        }
    }
}
