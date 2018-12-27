using Xunit;
using Xunit.Abstractions;
using System.Linq;
using EfCore.InMemoryHelpers;
using Microsoft.EntityFrameworkCore;

public class ContextBuilderTests : TestBase
{
    [Fact]
    public void GetInMemoryContext()
    {
        using (var context = InMemoryContextBuilder.Build<TestDataContext>())
        {
            var entity = new TestEntity
            {
                Property = "prop"
            };
            context.Add(entity);
            context.SaveChanges();
            var item = context.TestEntities.ToList();
            Assert.Single(item);
        }
    }

    public ContextBuilderTests(ITestOutputHelper output) :
        base(output)
    {
    }

    public class TestEntity
    {
        public int Id { get; set; }
        public string Property { get; set; }
    }

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
        }
    }
}