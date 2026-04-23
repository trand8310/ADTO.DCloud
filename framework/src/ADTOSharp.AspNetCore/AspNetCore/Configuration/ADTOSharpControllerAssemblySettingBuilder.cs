using System;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace ADTOSharp.AspNetCore.Configuration;

public class ADTOSharpControllerAssemblySettingBuilder : IADTOSharpControllerAssemblySettingBuilder
{
    private readonly ADTOSharpControllerAssemblySetting _setting;

    public ADTOSharpControllerAssemblySettingBuilder(ADTOSharpControllerAssemblySetting setting)
    {
        _setting = setting;
    }

    public ADTOSharpControllerAssemblySettingBuilder Where(Func<Type, bool> predicate)
    {
        _setting.TypePredicate = predicate;
        return this;
    }

    public ADTOSharpControllerAssemblySettingBuilder ConfigureControllerModel(Action<ControllerModel> configurer)
    {
        _setting.ControllerModelConfigurer = configurer;
        return this;
    }
}