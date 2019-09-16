using ApprovalTests;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace EfCore.InMemoryHelpers.Test
{
    public class ConcurrencyWithStringTests : TestBase
    {
        public ConcurrencyWithStringTests(ITestOutputHelper output)
            :
            base(output)
        { }

        [Fact]
        public void NullConflictThrows()
        {
            using (var context = InMemoryContextBuilder.Build<TestDataContext>())
            {
                var entity = new TestEntity
                {
                    Property = "prop",
                    ConcurrencyToken = "573dfb98237e4f51bafaab0317777960"
                };
                context.Add(entity);
                context.SaveChanges();
                var update = new TestEntity
                {
                    Id = entity.Id,
                    Property = "Something new",
                    ConcurrencyToken = "4b39590ea0894ab9a05912456bf700b6"
                };
                context.Entry(update).Property("Property").IsModified = true;
                var exception = Assert.Throws<DbUpdateConcurrencyException>(() => context.SaveChanges());
                Approvals.Verify(exception.Message);
            }
        }

        [Fact]
        public void SetConcurrencyTokenToNullThrows()
        {
            using (var context = InMemoryContextBuilder.Build<TestDataContext>())
            {
                var entity = new TestEntity
                {
                    Property = "prop",
                    ConcurrencyToken = "616e2ca1a52a4c57afbfe80ecc54125c"
                };
                context.Add(entity);
                context.SaveChanges();
                var update = new TestEntity
                {
                    Id = entity.Id,
                    Property = "Something new",
                    ConcurrencyToken = null
                };
                context.Entry(update).Property("Property").IsModified = true;
                var exception = Assert.Throws<DbUpdateConcurrencyException>(() => context.SaveChanges());
                Approvals.Verify(exception.Message);
            }
        }

        [Fact]
        public void UpdateMultipleSameEntity()
        {
            using (var context = InMemoryContextBuilder.Build<TestDataContext>())
            {
                var entity1 = new TestEntity
                {
                    Property = "prop",
                    ConcurrencyToken = "3c615ac3734046bc8bf6fd58d979c1ae"
                };
                var entity2 = new TestEntity
                {
                    Property = "prop",
                    ConcurrencyToken = "cc4c1ce921e6457b89d00c7c2c6de2a9"
                };
                context.AddRange(entity1, entity2);
                context.SaveChanges();
                entity1.Property = "Something new";
                entity2.Property = "Something new";
                context.SaveChanges();
            }
        }

        [Fact]
        public void UpdateSameEntity()
        {
            using (var context = InMemoryContextBuilder.Build<TestDataContext>())
            {
                var entity = new TestEntity
                {
                    Property = "prop",
                    ConcurrencyToken = "28f4fc79da8f43b887d2eef4e82a2be5"
                };
                context.Add(entity);
                context.SaveChanges();
                entity.Property = "Something new";
                context.SaveChanges();
            }
        }

        [Fact]
        public void UpdateSucceeds()
        {
            using (var context = InMemoryContextBuilder.Build<TestDataContext>())
            {
                var entity = new TestEntity
                {
                    Property = "prop",
                    ConcurrencyToken = "d6f321a1320846a49b81595c4d41d25d"
                };
                context.Add(entity);
                context.SaveChanges();
                var firstToken = entity.ConcurrencyToken;
                var update = new TestEntity
                {
                    Id = entity.Id,
                    Property = "Something new",
                    ConcurrencyToken = firstToken
                };
                context.Entry(update).Property("Property").IsModified = true;
                context.SaveChanges();
                Assert.Equal(firstToken, update.ConcurrencyToken);
            }
        }

        [Fact]
        public void ValueConflictThrows()
        {
            using (var context = InMemoryContextBuilder.Build<TestDataContext>())
            {
                var entity = new TestEntity
                {
                    Property = "prop",
                    ConcurrencyToken = "19ec62bea4f94ef1a8e4723707d3cc85"
                };
                context.Add(entity);
                context.SaveChanges();
                var update = new TestEntity
                {
                    Id = entity.Id,
                    Property = "Something new",
                    ConcurrencyToken = "dbd48fc43d9244f7a08cd54bae94b15a"
                };
                context.Entry(update).Property("Property").IsModified = true;
                var exception = Assert.Throws<DbUpdateConcurrencyException>(() => context.SaveChanges());
                Approvals.Verify(exception.Message);
            }
        }

        public class TestEntity
        {
            public int Id { get; set; }
            public string Property { get; set; }
            public string ConcurrencyToken { get; set; }
        }

        private class TestDataContext : DbContext
        {
            public TestDataContext(DbContextOptions options)
                : base(options)
            { }

            public DbSet<TestEntity> TestEntities { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                var entity = modelBuilder.Entity<TestEntity>();
                //            entity.HasKey(p => p.Id);
                entity.Property(p => p.ConcurrencyToken)
                    .IsConcurrencyToken();
            }
        }
    }
}