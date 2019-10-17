#pragma warning disable EF1001 // Internal EF Core API usage.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EfCore.InMemoryHelpers
{
    internal class StateManagerWrapper : IStateManager
    {
        private readonly ConcurrencyValidator concurrencyValidator;
        private readonly IStateManager inner;

        public StateManagerWrapper(IStateManager stateManager)
        {
            inner = stateManager;
            concurrencyValidator = new ConcurrencyValidator();
        }

        public event EventHandler<EntityStateChangedEventArgs> StateChanged
        {
            add => inner.StateChanged += value;
            remove => inner.StateChanged -= value;
        }

        public event EventHandler<EntityTrackedEventArgs> Tracked
        {
            add => inner.Tracked += value;
            remove => inner.Tracked -= value;
        }

        public CascadeTiming CascadeDeleteTiming { get => inner.CascadeDeleteTiming; set => inner.CascadeDeleteTiming = value; }

        public int ChangedCount
        {
            get => inner.ChangedCount;
            set => inner.ChangedCount = value;
        }

        public DbContext Context => inner.Context;

        public int Count => inner.Count;

        public CascadeTiming DeleteOrphansTiming { get => inner.DeleteOrphansTiming; set => inner.DeleteOrphansTiming = value; }

        public StateManagerDependencies Dependencies => inner.Dependencies;

        public IEntityMaterializerSource EntityMaterializerSource => inner.EntityMaterializerSource;

        IEntityMaterializerSource IStateManager.EntityMaterializerSource => inner.EntityMaterializerSource;

        public IEnumerable<InternalEntityEntry> Entries => inner.Entries;

        public IInternalEntityEntryNotifier InternalEntityEntryNotifier => inner.InternalEntityEntryNotifier;

        public IModel Model => inner.Model;

        public bool SensitiveLoggingEnabled => inner.SensitiveLoggingEnabled;

        public IDiagnosticsLogger<DbLoggerCategory.Update> UpdateLogger => inner.UpdateLogger;

        public IValueGenerationManager ValueGenerationManager => inner.ValueGenerationManager;

        public void AcceptAllChanges()
        {
            inner.AcceptAllChanges();
        }

        public void CascadeChanges(bool force)
        {
            inner.CascadeChanges(force);
        }

        public void CascadeDelete(InternalEntityEntry entry, bool force, IEnumerable<IForeignKey> foreignKeys = null)
        {
            inner.CascadeDelete(entry, force, foreignKeys);
        }

        public IEntityFinder CreateEntityFinder(IEntityType entityType)
        {
            return inner.CreateEntityFinder(entityType);
        }

        public InternalEntityEntry CreateEntry(IDictionary<string, object> values, IEntityType entityType)
        {
            return inner.CreateEntry(values, entityType);
        }

        public InternalEntityEntry FindPrincipal(InternalEntityEntry dependentEntry, IForeignKey foreignKey)
        {
            return inner.FindPrincipal(dependentEntry, foreignKey);
        }

        public InternalEntityEntry FindPrincipalUsingPreStoreGeneratedValues(InternalEntityEntry dependentEntry, IForeignKey foreignKey)
        {
            return inner.FindPrincipalUsingPreStoreGeneratedValues(dependentEntry, foreignKey);
        }

        public InternalEntityEntry FindPrincipalUsingRelationshipSnapshot(InternalEntityEntry dependentEntry, IForeignKey foreignKey)
        {
            return inner.FindPrincipalUsingRelationshipSnapshot(dependentEntry, foreignKey);
        }

        public int GetCountForState(bool added = false, bool modified = false, bool deleted = false, bool unchanged = false)
        {
            return inner.GetCountForState(added, modified, deleted, unchanged);
        }

        public IEnumerable<InternalEntityEntry> GetDependents(InternalEntityEntry principalEntry, IForeignKey foreignKey)
        {
            return inner.GetDependents(principalEntry, foreignKey);
        }

        public IEnumerable<InternalEntityEntry> GetDependentsFromNavigation(InternalEntityEntry principalEntry, IForeignKey foreignKey)
        {
            return inner.GetDependentsFromNavigation(principalEntry, foreignKey);
        }

        public IEnumerable<InternalEntityEntry> GetDependentsUsingRelationshipSnapshot(InternalEntityEntry principalEntry, IForeignKey foreignKey)
        {
            return inner.GetDependentsUsingRelationshipSnapshot(principalEntry, foreignKey);
        }

        public IEnumerable<InternalEntityEntry> GetEntriesForState(bool added = false, bool modified = false, bool deleted = false, bool unchanged = false)
        {
            return inner.GetEntriesForState(added, modified, deleted, unchanged);
        }

        public IList<IUpdateEntry> GetEntriesToSave(bool cascadeChanges)
        {
            return inner.GetEntriesToSave(cascadeChanges);
        }

        public IEnumerable<TEntity> GetNonDeletedEntities<TEntity>() where TEntity : class
        {
            return inner.GetNonDeletedEntities<TEntity>();
        }

        public InternalEntityEntry GetOrCreateEntry(object entity)
        {
            return inner.GetOrCreateEntry(entity);
        }

        public InternalEntityEntry GetOrCreateEntry(object entity, IEntityType entityType)
        {
            return inner.GetOrCreateEntry(entity, entityType);
        }

        public IEnumerable<Tuple<INavigation, InternalEntityEntry>> GetRecordedReferrers(object referencedEntity, bool clear)
        {
            return inner.GetRecordedReferrers(referencedEntity, clear);
        }

        public void OnStateChanged(InternalEntityEntry internalEntityEntry, EntityState oldState)
        {
            inner.OnStateChanged(internalEntityEntry, oldState);
        }

        public void OnTracked(InternalEntityEntry internalEntityEntry, bool fromQuery)
        {
            inner.OnTracked(internalEntityEntry, fromQuery);
        }

        public void RecordReferencedUntrackedEntity(object referencedEntity, INavigation navigation, InternalEntityEntry referencedFromEntry)
        {
            inner.RecordReferencedUntrackedEntity(referencedEntity, navigation, referencedFromEntry);
        }

        public void ResetState()
        {
            inner.ResetState();
        }

        public Task ResetStateAsync(CancellationToken cancellationToken = default)
        {
            return inner.ResetStateAsync(cancellationToken);
        }

        public int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            inner.Context.ValidateIndexes();
            concurrencyValidator.ValidateIndexes(inner.Context);
            return inner.SaveChanges(acceptAllChangesOnSuccess);
        }

        public Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellation = default)
        {
            inner.Context.ValidateIndexes();
            concurrencyValidator.ValidateIndexes(inner.Context);
            return inner.SaveChangesAsync(acceptAllChangesOnSuccess, cancellation);
        }

        public InternalEntityEntry StartTracking(InternalEntityEntry entry)
        {
            return inner.StartTracking(entry);
        }

        public InternalEntityEntry StartTrackingFromQuery(IEntityType baseEntityType, object entity, in ValueBuffer valueBuffer)
        {
            return inner.StartTrackingFromQuery(baseEntityType, entity, valueBuffer);
        }

        public void StateChanging(InternalEntityEntry entry, EntityState newState)
        {
            inner.StateChanging(entry, newState);
        }

        public void StopTracking(InternalEntityEntry entry, EntityState oldState)
        {
            inner.StopTracking(entry, oldState);
        }

        public InternalEntityEntry TryGetEntry(IKey key, object[] keyValues)
        {
            return inner.TryGetEntry(key, keyValues);
        }

        public InternalEntityEntry TryGetEntry(object entity, bool throwOnNonUniqueness = true)
        {
            return inner.TryGetEntry(entity, throwOnNonUniqueness);
        }

        public InternalEntityEntry TryGetEntry(object entity, IEntityType type)
        {
            return inner.TryGetEntry(entity, type);
        }

        public InternalEntityEntry TryGetEntry(IKey key, object[] keyValues, bool throwOnNullKey, out bool hasNullKey)
        {
            return inner.TryGetEntry(key, keyValues, throwOnNullKey, out hasNullKey);
        }

        public InternalEntityEntry TryGetEntry(object entity, IEntityType type, bool throwOnTypeMismatch = true)
        {
            return inner.TryGetEntry(entity, type, throwOnTypeMismatch);
        }

        public void Unsubscribe()
        {
            inner.Unsubscribe();
        }

        public void UpdateDependentMap(InternalEntityEntry entry, IForeignKey foreignKey)
        {
            inner.UpdateDependentMap(entry, foreignKey);
        }

        public void UpdateIdentityMap(InternalEntityEntry entry, IKey principalKey)
        {
            inner.UpdateIdentityMap(entry, principalKey);
        }
    }
}
#pragma warning restore EF1001 // Internal EF Core API usage.
