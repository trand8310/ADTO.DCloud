using System;
using System.Linq.Expressions;
using ADTOSharp.Timing;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ADTOSharp.EntityFrameworkCore.ValueConverters;

public class ADTOSharpDateTimeValueConverter : ValueConverter<DateTime?, DateTime?>
{
    public ADTOSharpDateTimeValueConverter([CanBeNull] ConverterMappingHints mappingHints = null)
        : base(Normalize, Normalize, mappingHints)
    {
    }

    private static readonly Expression<Func<DateTime?, DateTime?>> Normalize = x =>
        x.HasValue ? Clock.Normalize(x.Value) : x;
}