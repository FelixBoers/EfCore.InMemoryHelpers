using System;
using System.Linq;
using System.Threading.Tasks;
using ApprovalTests;
using Microsoft.EntityFrameworkCore;
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

        [Fact]
        public void UniqueIndexReUsedContextThrows()
        {
            const string FAKE_DB_NAME = "FAKE-DB-NAME-1";
            var context1 = InMemoryContextBuilder.Build<TestDataContext>(FAKE_DB_NAME);

            var entity1 = new TestEntity
            {
                Property = "prop"
            };
            context1.Add(entity1);
            context1.SaveChanges();

            var context2 = InMemoryContextBuilder.Build<TestDataContext>(FAKE_DB_NAME);

            var user2 = new TestEntity
            {
                Property = "prop"
            };
            context2.Add(user2);
            var exception = Assert.Throws<Exception>(() => context2.SaveChanges());

            context1.Dispose();
            context2.Dispose();
        }

        [Fact]
        public async Task UniqueIndexReUsedContextWithConcurrencyThrows()
        {
            const string FAKE_DB_NAME = "FAKE-DB-NAME-CONCURRENCY";
            const int TEST_CASES = 10;

            for (var i = 0; i < TEST_CASES; i++)
            {
                var builder = new DbContextOptionsBuilder<TestDataContext>();

                TestDataContext GetContext() => InMemoryContextBuilder.Build<TestDataContext>(FAKE_DB_NAME);

                var context = GetContext();

                var task1 = Task.Run(async () =>
                {
                    var newContext = GetContext();
                    var entity1 = new TestEntity
                    {
                        Property = "prop"
                    };
                    newContext.Add(entity1);
                    await newContext.SaveChangesAsync();
                    newContext.Dispose();
                });
                var task2 = Task.Run(async () =>
                {
                    var newContext = GetContext();
                    var entity2 = new TestEntity
                    {
                        Property = "prop"
                    };
                    newContext.Add(entity2);
                    await newContext.SaveChangesAsync();
                    newContext.Dispose();
                });

                await Assert.ThrowsAsync<Exception>(async () => await Task.WhenAll(new[] { task1, task2 }));
            }
        }

        [Fact]
        public void UniqueIndexReUsedContextWhenEditingThrows()
        {
            const string FAKE_DB_NAME = "FAKE-DB-NAME-2";
            var context1 = InMemoryContextBuilder.Build<TestDataContext>(FAKE_DB_NAME);

            var entity1 = new TestEntity
            {
                Property = "prop1"
            };
            context1.Add(entity1);
            var user2 = new TestEntity
            {
                Property = "prop2"
            };
            context1.Add(user2);
            context1.SaveChanges();

            var context2 = InMemoryContextBuilder.Build<TestDataContext>(FAKE_DB_NAME);
            var editingItem = context2.Set<TestEntity>().Where(it => it.Property == "prop2").Single();
            editingItem.Property = "prop1";

            var exception = Assert.Throws<Exception>(() => context2.SaveChanges());

            context1.Dispose();
            context2.Dispose();
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
                testEntity.Property(b => b.Property)
                    .IsRequired();

                testEntity.HasIndex(u => u.Property)
                    .IsUnique();

                var testEntitySameTypes = modelBuilder.Entity<TestEntityUnique>();
                testEntitySameTypes.HasIndex(u => new { u.A, u.B }).IsUnique();
            }
        }
    }
}