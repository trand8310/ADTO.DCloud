using ADTO.DCloud.Authentication.External;
using ADTOSharp.AutoMapper;
using System.Collections.Generic;
 

namespace ADTO.DCloud.Web.Models.TokenAuth;

[AutoMapFrom(typeof(ExternalLoginProviderInfo))]
public class ExternalLoginProviderInfoModel
{
    public string Name { get; set; }

    public string ClientId { get; set; }

    public Dictionary<string, string> AdditionalParams { get; set; }

}
