using Xunit;
using Xunit.Abstractions;
using EfCore.InMemoryHelpers;
using Microsoft.EntityFrameworkCore;

public class QueryTypeTest : TestBase
{
    [Fact]
    public void WithQueryTypeShouldNotThrow()
    {
        using (var context = InMemoryContextBuilder.Build<TestDataContext>())
        {
            var entity = new TestEntity
            {
                Property = "prop"
            };
            context.Add(entity);
            context.SaveChanges();
        }
    }

    public class TestEntity
    {
        public int Id { get; set; }
        public string Property { get; set; }
    }
    public class TestEntityCount
    {
        public string Property { get; set; }
        public int Count { get; set; }
    }

    class TestDataContext : DbContext
    {
        public DbSet<TestEntity> TestEntities { get; set; }

        public TestDataContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Query<TestEntityCount>().ToView("View_BlogPostCounts")
                .Property(v => v.Property).HasColumnName("Property");
            var entity = modelBuilder.Entity<TestEntity>();
            entity.Property(b => b.Property)
                .IsRequired();
        }
    }

    public QueryTypeTest(ITestOutputHelper output) :
        base(output)
    {
    }
}