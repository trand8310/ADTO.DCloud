using System;
using System.Collections.Generic;

namespace ADTOSharp.Modules
{
    public interface IADTOSharpModuleManager
    {
        ADTOSharpModuleInfo StartupModule { get; }

        IReadOnlyList<ADTOSharpModuleInfo> Modules { get; }

        void Initialize(Type startupModule);

        void StartModules();

        void ShutdownModules();
    }
}