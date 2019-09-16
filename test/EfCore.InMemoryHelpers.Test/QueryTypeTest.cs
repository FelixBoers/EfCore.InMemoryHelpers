using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace EfCore.InMemoryHelpers.Test
{
    public class QueryTypeTest : TestBase
    {
        public QueryTypeTest(ITestOutputHelper output)
            :
            base(output)
        { }

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

        private class TestDataContext : DbContext
        {
            public TestDataContext(DbContextOptions options)
                : base(options)
            { }

            public DbSet<TestEntity> TestEntities { get; set; }

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
    }
}