using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;

//TODO: remove when this is fixed https://github.com/aspnet/EntityFrameworkCore/issues/6872
static class InMemoryValueResetter
{
    public static void ResetValueGenerators(this DbContext context)
    {
        var cache = context.GetService<IValueGeneratorCache>();

        foreach (var keyProperty in context.Model.GetEntityTypes()
            .Where(x => !x.IsQueryType)
            .Select(e => e.FindPrimaryKey().Properties[0])
            .Where(p => p.ClrType == typeof(int)
                        && p.ValueGenerated == ValueGenerated.OnAdd))
        {
            var generator = (ResettableValueGenerator) cache.GetOrAdd(
                keyProperty,
                keyProperty.DeclaringEntityType,
                (p, e) => new ResettableValueGenerator());

            generator.Reset();
        }
    }

    class ResettableValueGenerator : ValueGenerator<int>
    {
        int current;

        public override bool GeneratesTemporaryValues => false;

        public override int Next(EntityEntry entry) => Interlocked.Increment(ref current);

        public void Reset() => current = 0;
    }
}