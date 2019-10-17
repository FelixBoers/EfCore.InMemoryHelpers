using ApprovalTests;
using Microsoft.EntityFrameworkCore;
using System;
using Xunit;
using Xunit.Abstractions;

namespace EfCore.InMemoryHelpers.Test
{
    public class UniqueIndexTests : TestBase
    {
        public UniqueIndexTests(ITestOutputHelper output)
            :
            base(output)
        { }

        [Fact]
        public void RespectsUniqueIndexOrder()
        {
            using (var context = InMemoryContextBuilder.Build<TestDataContext>())
            {
                var entity1 = new TestEntityUnique { A = "a", B = "b" };
                var entity2 = new TestEntityUnique { A = "b", B = "a" };
                context.AddRange(entity1, entity2);
                context.SaveChanges();
            }
        }

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

        public class TestEntity
        {
            public int Id { get; set; }

            public string Property { get; set; }
        }

        public class TestEntityUnique
        {
            public string A { get; set; }
            
            public string B { get; set; }
            
            public int Id { get; set; }
        }

        private class TestDataContext : DbContext
        {
            public TestDataContext(DbContextOptions options)
                : base(options)
            { }

            public DbSet<TestEntity> TestEntities { get; set; }
            
            public DbSet<TestEntityUnique> TestEntityUnique { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                var testEntity = modelBuilder.Entity<TestEntity>();
                testEntity.HasKey(p => p.Id);

                testEntity.Property(p => p.Id)
                    .ValueGeneratedOnAdd();

                testEntity.Property(b => b.Property)
                    .IsRequired();

                testEntity.HasIndex(u => u.Property)
                    .IsUnique();

                var testEntitySameTypes = modelBuilder.Entity<TestEntityUnique>();
                testEntitySameTypes.HasKey(p => p.Id);

                testEntitySameTypes.Property(p => p.Id)
                    .ValueGeneratedOnAdd();
                testEntitySameTypes.HasIndex(u => new { u.A, u.B }).IsUnique();
            }
        }
    }
}