using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
// ReSharper disable IdentifierTypo

class StateManagerWrapper : IStateManager
{
    IStateManager inner;
    ConcurrencyValidator concurrencyValidator;

    public StateManagerWrapper(IStateManager stateManager)
    {
        inner = stateManager;
        concurrencyValidator = new ConcurrencyValidator();
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

    public void ResetState()
    {
        inner.ResetState();
    }

    public InternalEntityEntry GetOrCreateEntry(object entity)
    {
        return inner.GetOrCreateEntry(entity);
    }

    public InternalEntityEntry GetOrCreateEntry(object entity, IEntityType entityType)
    {
        return inner.GetOrCreateEntry(entity, entityType);
    }

    public InternalEntityEntry CreateEntry(IDictionary<string, object> values, IEntityType entityType)
    {
        return inner.CreateEntry(values, entityType);
    }

    public InternalEntityEntry StartTrackingFromQuery(IEntityType baseEntityType, object entity, in ValueBuffer valueBuffer, ISet<IForeignKey> handledForeignKeys)
    {
        return inner.StartTrackingFromQuery(baseEntityType, entity, in valueBuffer, handledForeignKeys);
    }

    public void BeginTrackingQuery()
    {
        inner.BeginTrackingQuery();
    }

    public InternalEntityEntry TryGetEntry(IKey key, object[] keyValues)
    {
        return inner.TryGetEntry(key, keyValues);
    }

    public InternalEntityEntry TryGetEntry(IKey key, in ValueBuffer valueBuffer, bool throwOnNullKey)
    {
        return inner.TryGetEntry(key, valueBuffer, throwOnNullKey);
    }

    public InternalEntityEntry TryGetEntry(object entity, bool throwOnNonUniqueness = true)
    {
        return inner.TryGetEntry(entity, throwOnNonUniqueness);
    }

    public InternalEntityEntry TryGetEntry(object entity, IEntityType type)
    {
        return inner.TryGetEntry(entity, type);
    }

    public InternalEntityEntry StartTracking(InternalEntityEntry entry)
    {
        return inner.StartTracking(entry);
    }

    public void StopTracking(InternalEntityEntry entry)
    {
        inner.StopTracking(entry);
    }

    public void RecordReferencedUntrackedEntity(object referencedEntity, INavigation navigation, InternalEntityEntry referencedFromEntry)
    {
        inner.RecordReferencedUntrackedEntity(referencedEntity,navigation, referencedFromEntry);
    }

    public IEnumerable<Tuple<INavigation, InternalEntityEntry>> GetRecordedReferrers(object referencedEntity, bool clear)
    {
        return inner.GetRecordedReferrers(referencedEntity, clear);
    }

    public InternalEntityEntry GetPrincipal(InternalEntityEntry dependentEntry, IForeignKey foreignKey)
    {
        return inner.GetPrincipal(dependentEntry, foreignKey);
    }

    public InternalEntityEntry GetPrincipalUsingPreStoreGeneratedValues(InternalEntityEntry dependentEntry, IForeignKey foreignKey)
    {
        return inner.GetPrincipalUsingPreStoreGeneratedValues(dependentEntry, foreignKey);
    }

    public InternalEntityEntry GetPrincipalUsingRelationshipSnapshot(InternalEntityEntry dependentEntry, IForeignKey foreignKey)
    {
        return inner.GetPrincipalUsingRelationshipSnapshot(dependentEntry, foreignKey);
    }

    public void UpdateIdentityMap(InternalEntityEntry entry, IKey principalKey)
    {
        inner.UpdateIdentityMap(entry, principalKey);
    }

    public void UpdateDependentMap(InternalEntityEntry entry, IForeignKey foreignKey)
    {
        inner.UpdateDependentMap(entry, foreignKey);
    }

    public IEnumerable<InternalEntityEntry> GetDependentsFromNavigation(InternalEntityEntry principalEntry, IForeignKey foreignKey)
    {
        return inner.GetDependentsFromNavigation(principalEntry, foreignKey);
    }

    public IEnumerable<InternalEntityEntry> GetDependents(InternalEntityEntry principalEntry, IForeignKey foreignKey)
    {
        return inner.GetDependents(principalEntry, foreignKey);
    }

    public IEnumerable<InternalEntityEntry> GetDependentsUsingRelationshipSnapshot(InternalEntityEntry principalEntry, IForeignKey foreignKey)
    {
        return inner.GetDependentsUsingRelationshipSnapshot(principalEntry, foreignKey);
    }

    public IReadOnlyList<IUpdateEntry> GetEntriesToSave()
    {
        return inner.GetEntriesToSave();
    }

    public void AcceptAllChanges()
    {
        inner.AcceptAllChanges();
    }

    public IEntityFinder CreateEntityFinder(IEntityType entityType)
    {
        return   inner.CreateEntityFinder(entityType);
    }

    public TrackingQueryMode GetTrackingQueryMode(IEntityType entityType)
    {
        return inner.GetTrackingQueryMode(entityType);
    }

    public void EndSingleQueryMode()
    {
        inner.EndSingleQueryMode();
    }

    public void Unsubscribe()
    {
        inner.Unsubscribe();
    }

    public void OnTracked(InternalEntityEntry internalEntityEntry, bool fromQuery)
    {
        inner.OnTracked(internalEntityEntry, fromQuery);
    }

    public void OnStateChanged(InternalEntityEntry internalEntityEntry, EntityState oldState)
    {
        inner.OnStateChanged(internalEntityEntry, oldState);
    }

    public IEnumerable<InternalEntityEntry> Entries => inner.Entries;

    public int ChangedCount
    {
        get =>  inner.ChangedCount;
        set => inner.ChangedCount = value;
    }

    public IInternalEntityEntryNotifier InternalEntityEntryNotifier => inner.InternalEntityEntryNotifier;
    public IValueGenerationManager ValueGenerationManager => inner.ValueGenerationManager;
    public DbContext Context => inner.Context;
    public IEntityMaterializerSource EntityMaterializerSource => inner.EntityMaterializerSource;
    public bool SensitiveLoggingEnabled => inner.SensitiveLoggingEnabled;
    public IDiagnosticsLogger<DbLoggerCategory.Update> UpdateLogger => inner.UpdateLogger;
    public event EventHandler<EntityTrackedEventArgs> Tracked
    {
        add => inner.Tracked+=value;
        remove => inner.Tracked -= value;
    }

    public event EventHandler<EntityStateChangedEventArgs> StateChanged
    {
        add => inner.StateChanged += value;
        remove => inner.StateChanged -= value;
    }

}