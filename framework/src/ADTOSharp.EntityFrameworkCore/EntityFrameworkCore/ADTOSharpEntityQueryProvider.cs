using System.Linq.Expressions;
using System.Threading;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace ADTOSharp.EntityFrameworkCore;

#pragma warning disable EF1001
public class ADTOSharpEntityQueryProvider : EntityQueryProvider
{
    protected ADTOSharpEfCoreCurrentDbContext ADTOSharpEfCoreCurrentDbContext { get; }
    protected ICurrentDbContext CurrentDbContext { get; }

    public ADTOSharpEntityQueryProvider(
        IQueryCompiler queryCompiler,
        ADTOSharpEfCoreCurrentDbContext efCoreCurrentDbContext,
        ICurrentDbContext currentDbContext)
        : base(queryCompiler)
    {
        ADTOSharpEfCoreCurrentDbContext = efCoreCurrentDbContext;
        CurrentDbContext = currentDbContext;
    }

    public override object Execute(Expression expression)
    {
        using (ADTOSharpEfCoreCurrentDbContext.Use(CurrentDbContext.Context as ADTOSharpDbContext))
        {
            return base.Execute(expression);
        }
    }

    public override TResult Execute<TResult>(Expression expression)
    {
        using (ADTOSharpEfCoreCurrentDbContext.Use(CurrentDbContext.Context as ADTOSharpDbContext))
        {
            return base.Execute<TResult>(expression);
        }
    }

    public override TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = new CancellationToken())
    {
        using (ADTOSharpEfCoreCurrentDbContext.Use(CurrentDbContext.Context as ADTOSharpDbContext))
        {
            return base.ExecuteAsync<TResult>(expression, cancellationToken);
        }
    }
}
#pragma warning restore EF1001