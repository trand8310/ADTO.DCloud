using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ADTOSharp.Modules;

namespace ADTOSharp.Reflection
{
    public class ADTOSharpAssemblyFinder : IAssemblyFinder
    {
        private readonly IADTOSharpModuleManager _moduleManager;

        public ADTOSharpAssemblyFinder(IADTOSharpModuleManager moduleManager)
        {
            _moduleManager = moduleManager;
        }

        public List<Assembly> GetAllAssemblies()
        {
            var assemblies = new List<Assembly>();

            foreach (var module in _moduleManager.Modules)
            {
                assemblies.Add(module.Assembly);
                assemblies.AddRange(module.Instance.GetAdditionalAssemblies());
            }

            return assemblies.Distinct().ToList();
        }
    }
}