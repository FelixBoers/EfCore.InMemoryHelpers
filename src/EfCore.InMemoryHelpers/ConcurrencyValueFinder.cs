﻿#pragma warning disable EF1001 // Internal EF Core API usage.
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EfCore.InMemoryHelpers
{
    internal static class ConcurrencyValueFinder
    {
        public static bool GetConcurrency(this IEntityType entityType, out Action<object, byte[]> setter, out Func<object, byte[]> getter)
        {
            var concurrencyProperty = entityType
                .GetProperties()
                .SingleOrDefault(x => x.IsConcurrencyToken && x.ValueGenerated == ValueGenerated.OnAddOrUpdate);
            if (concurrencyProperty == null)
            {
                setter = null;
                getter = null;
                return false;
            }

            setter = (z, y) =>
            {
                var valueConverter = concurrencyProperty.GetValueConverter();
                var propertySetter = concurrencyProperty.AsPropertyBase().Setter;
                if (valueConverter == null)
                {
                    propertySetter.SetClrValue(z, y);
                }
                else
                {
                    var value = valueConverter.ConvertFromProvider(y);
                    propertySetter.SetClrValue(z, value);
                }
            };
            getter = o =>
            {
                var clrValue = concurrencyProperty.GetGetter().GetClrValue(o);
                var valueConverter = concurrencyProperty.GetValueConverter();
                if (valueConverter == null)
                {
                    return (byte[])clrValue;
                }

                var bytes = (byte[])valueConverter.ConvertToProvider(clrValue);
                if (bytes.All(x => x == 0))
                {
                    return null;
                }

                return bytes;
            };
            return true;
        }
    }
}
#pragma warning restore EF1001 // Internal EF Core API usage.