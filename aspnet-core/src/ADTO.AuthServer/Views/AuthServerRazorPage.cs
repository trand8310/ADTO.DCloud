using ADTOSharp.AspNetCore.Mvc.Views;
using ADTOSharp.Runtime.Session;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using ADTO.DCloud;

namespace ADTO.AuthServer.Views
{
    public abstract class AuthServerRazorPage<TModel> : ADTOSharpRazorPage<TModel>
    {
        [RazorInject]
        public IADTOSharpSession ADTOSession { get; set; }

        protected AuthServerRazorPage()
        {
            LocalizationSourceName = DCloudConsts.LocalizationSourceName;
        }
    }
}

