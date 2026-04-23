using System;
using System.Threading;

namespace ADTOSharp.EntityFrameworkCore;

public class ADTOSharpEfCoreCurrentDbContext
{
    private readonly AsyncLocal<ADTOSharpDbContext> _current = new AsyncLocal<ADTOSharpDbContext>();

    public ADTOSharpDbContext Context => _current.Value;

    public IDisposable Use(ADTOSharpDbContext context)
    {
        var previousValue = Context;
        _current.Value = context;
        return new DisposeAction(() =>
        {
            _current.Value = previousValue;
        });
    }
}