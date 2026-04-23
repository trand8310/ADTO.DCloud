using ADTOSharp;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.OpenIddict
{
    public class PermissionValueProviderInfo
    {
        public string Name { get; }

        public string Key { get; }

        public PermissionValueProviderInfo([NotNull] string name, [NotNull] string key)
        {
            Check.NotNull(name, nameof(name));
            Check.NotNull(key, nameof(key));

            Name = name;
            Key = key;
        }
    }

}
