#pragma warning disable EF1001 // Internal EF Core API usage.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace EfCore.InMemoryHelpers
{
    internal class DbContextDependenciesEx : IDbContextDependencies
    {
        private readonly DbContextDependencies inner;

        public DbContextDependenciesEx(ICurrentDbContext currentContext, IChangeDetector changeDetector, IDbSetSource setSource, IEntityFinderSource entityFinderSource, IEntityGraphAttacher entityGraphAttacher, IModel model, IAsyncQueryProvider queryProvider, IStateManager stateManager, IDiagnosticsLogger<DbLoggerCategory.Update> updateLogger, IDiagnosticsLogger<DbLoggerCategory.Infrastructure> infrastructureLogger)
        {
            inner = new DbContextDependencies(currentContext, changeDetector, setSource, entityFinderSource, entityGraphAttacher, model, queryProvider, new StateManagerWrapper(stateManager), updateLogger, infrastructureLogger);
        }

        public IChangeDetector ChangeDetector => inner.ChangeDetector;
        public IEntityFinderFactory EntityFinderFactory => inner.EntityFinderFactory;
        public IEntityGraphAttacher EntityGraphAttacher => inner.EntityGraphAttacher;
        public IDiagnosticsLogger<DbLoggerCategory.Infrastructure> InfrastructureLogger => inner.InfrastructureLogger;
        public IModel Model => inner.Model;
        public IAsyncQueryProvider QueryProvider => inner.QueryProvider;
        public IDbSetSource SetSource => inner.SetSource;
        public IStateManager StateManager => inner.StateManager;
        public IDiagnosticsLogger<DbLoggerCategory.Update> UpdateLogger => inner.UpdateLogger;
    }
}
#pragma warning restore EF1001 // Internal EF Core API usage.
