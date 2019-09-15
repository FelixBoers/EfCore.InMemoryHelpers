using System;
using Xunit;
using Xunit.Abstractions;
using ApprovalTests;
using EfCore.InMemoryHelpers;
using Microsoft.EntityFrameworkCore;

public class UniqueIndexTests : TestBase
{
    [Fact]
    public void UniqueIndexThrows()
    {
        using (var context = InMemoryContextBuilder.Build<TestDataContext>())
        {
            var entity1 = new TestEntity
            {
                Property = "prop"
            };
            context.Add(entity1);
            var user2 = new TestEntity
            {
                Property = "prop"
            };
            context.Add(user2);
            var exception = Assert.Throws<Exception>(() => context.SaveChanges());
            Approvals.Verify(exception.Message);
        }
    }

    [Fact]
    public void RespectsUniqueIndexOrder()
    {
        using (var context = InMemoryContextBuilder.Build<TestDataContext>())
        {
            var entity1 = new TestEntityUnique {A = "a", B = "b"};
            var entity2 = new TestEntityUnique {A = "b", B = "a"};
            context.AddRange(entity1, entity2);
            context.SaveChanges();
        }
    }


    public UniqueIndexTests(ITestOutputHelper output) :
        base(output)
    {
    }

    public class TestEntityUnique
    {
        public int Id { get; set; }
        public string A { get; set; }
        public string B { get; set; }
    }

    public class TestEntity
    {
        public int Id { get; set; }
        public string Property { get; set; }
    }

    class TestDataContext : DbContext
    {
        public DbSet<TestEntity> TestEntities { get; set; }
        public DbSet<TestEntityUnique> TestEntityUnique { get; set; }

        public TestDataContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var testEntity = modelBuilder.Entity<TestEntity>();
            testEntity.Property(b => b.Property)
                .IsRequired();

            testEntity.HasIndex(u => u.Property)
                .IsUnique();

            var testEntitySameTypes = modelBuilder.Entity<TestEntityUnique>();
            testEntitySameTypes.HasIndex(u => new {u.A, u.B}).IsUnique();

        }
    }
}