using ADTOSharp;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.OpenIddict
{
    public class PermissionWithGrantedProviders
    {
        public string Name { get; }

        public bool IsGranted { get; set; }

        public List<PermissionValueProviderInfo> Providers { get; set; }

        public PermissionWithGrantedProviders([NotNull] string name, bool isGranted)
        {
            Check.NotNull(name, nameof(name));

            Name = name;
            IsGranted = isGranted;

            Providers = new List<PermissionValueProviderInfo>();
        }
    }

}
