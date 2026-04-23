using System;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace ADTOSharp.AspNetCore.Configuration;

public interface IADTOSharpControllerAssemblySettingBuilder
{
    ADTOSharpControllerAssemblySettingBuilder Where(Func<Type, bool> predicate);

    ADTOSharpControllerAssemblySettingBuilder ConfigureControllerModel(Action<ControllerModel> configurer);
}