using ADTOSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.OpenIddict
{
    public class MultiplePermissionWithGrantedProviders
    {
        public List<PermissionWithGrantedProviders> Result { get; }

        public MultiplePermissionWithGrantedProviders()
        {
            Result = new List<PermissionWithGrantedProviders>();
        }

        public MultiplePermissionWithGrantedProviders(string[] names)
        {
            Check.NotNull(names, nameof(names));

            Result = new List<PermissionWithGrantedProviders>();

            foreach (var name in names)
            {
                Result.Add(new PermissionWithGrantedProviders(name, false));
            }
        }
    }
}
