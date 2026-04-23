using System;
using System.Collections.Generic;
using AutoMapper;

namespace ADTOSharp.AutoMapper;

public interface IADTOSharpAutoMapperConfiguration
{
    List<Action<IMapperConfigurationExpression>> Configurators { get; }

    /// <summary>
    /// Use static Instance.
    /// Default: true.
    /// </summary>
    [Obsolete("Automapper will remove static API. See https://github.com/aspnetboilerplate/aspnetboilerplate/issues/4667")]
    bool UseStaticMapper { get; set; }
}