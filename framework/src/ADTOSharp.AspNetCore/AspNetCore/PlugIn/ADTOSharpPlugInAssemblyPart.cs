using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace ADTOSharp.AspNetCore.PlugIn;

public class ADTOSharpPlugInAssemblyPart : AssemblyPart, ICompilationReferencesProvider
{
    public ADTOSharpPlugInAssemblyPart(Assembly assembly)
        : base(assembly)
    {
    }

    IEnumerable<string> ICompilationReferencesProvider.GetReferencePaths() => Enumerable.Empty<string>();
}