using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace EfCore.InMemoryHelpers
{
    public static class InMemoryContextBuilder
    {
        public static TContext Build<TContext>()
            where TContext : DbContext
        {
            var builder = new DbContextOptionsBuilder<TContext>();
            return Build(builder);
        }

        public static TContext Build<TContext>(DbContextOptionsBuilder<TContext> builder)
            where TContext : DbContext
        {
            builder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            builder.ReplaceService<IDbContextDependencies, DbContextDependenciesEx>();
            Activator.CreateInstance(typeof(TContext), builder.Options);
            var dbContextOptions = (DbContextOptions) builder.Options;
            var dataContext = (TContext) Activator.CreateInstance(typeof(TContext), dbContextOptions);
            dataContext.ResetValueGenerators();
            return dataContext;
        }
    }
}