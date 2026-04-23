using System;
using ADTOSharp.Domain.Uow;
using Microsoft.AspNetCore.Http;

namespace ADTOSharp.AspNetCore.Uow;

public class UnitOfWorkMiddlewareOptions
{
    public Func<HttpContext, bool> Filter { get; set; } = context => true;

    public Func<HttpContext, UnitOfWorkOptions> OptionsFactory { get; set; } = context => new UnitOfWorkOptions();
}