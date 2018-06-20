using Microsoft.EntityFrameworkCore;

class TestDataContext : DbContext
{
    public DbSet<TestEntity> TestEntities { get; set; }

    public TestDataContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestEntity>()
            .Property(b => b.Property)
            .IsRequired();
    }
}