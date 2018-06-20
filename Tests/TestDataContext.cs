using Microsoft.EntityFrameworkCore;

class TestDataContext : DbContext
{
    public DbSet<TestEntity> TestEntities { get; set; }

    public TestDataContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<TestEntity>();
        entity.Property(b => b.Property)
            .IsRequired();

        entity.HasIndex(u => u.Property)
            .IsUnique();
    }
}