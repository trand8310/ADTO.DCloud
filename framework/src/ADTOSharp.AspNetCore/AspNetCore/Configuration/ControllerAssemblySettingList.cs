using System;
using System.Collections.Generic;
using System.Linq;
using ADTOSharp.Reflection.Extensions;
using JetBrains.Annotations;

namespace ADTOSharp.AspNetCore.Configuration;

public class ControllerAssemblySettingList : List<ADTOSharpControllerAssemblySetting>
{
    public List<ADTOSharpControllerAssemblySetting> GetSettings(Type controllerType)
    {
        return this.Where(controllerSetting => controllerSetting.Assembly == controllerType.GetAssembly()).ToList();
    }
}