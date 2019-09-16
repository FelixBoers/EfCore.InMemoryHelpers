using Microsoft.EntityFrameworkCore;

namespace EfCore.InMemoryHelpers.Test.Snippets
{
    internal class Sample
    {
        private void Simple()
        {
            #region simple

            using (var context = InMemoryContextBuilder.Build<MyDataContext>())
            {
                var entity = new MyEntity
                {
                    Property = "prop"
                };
                context.Add(entity);
                context.SaveChanges();
            }

            #endregion
        }

        private void WithBuilder()
        {
            #region withBuilder

            var builder = new DbContextOptionsBuilder<MyDataContext>();
            using (var context = InMemoryContextBuilder.Build<MyDataContext>(builder))
            {
                var entity = new MyEntity
                {
                    Property = "prop"
                };
                context.Add(entity);
                context.SaveChanges();
            }

            #endregion
        }

        private void WithConstructor()
        {
            #region customContextConstructor

            var builder = new DbContextOptionsBuilder<MyDataContext>();
            using (var context = InMemoryContextBuilder.Build(builder, options => new MyDataContext(options)))
            {
                var entity = new MyEntity
                {
                    Property = "prop"
                };
                context.Add(entity);
                context.SaveChanges();
            }

            #endregion
        }

        private void WithDatabaseName()
        {
            #region withDatabaseName

            var builder = new DbContextOptionsBuilder<MyDataContext>();
            using (var context = InMemoryContextBuilder.Build(builder, options => new MyDataContext(options)))
            {
                var entity = new MyEntity
                {
                    Property = "prop"
                };
                context.Add(entity);
                context.SaveChanges();
            }

            #endregion
        }
    }
}