using System.Collections.Generic;

namespace ADTOSharp.Web.Models.ADTOSharpUserConfiguration
{
    public class ADTOSharpUserAuthConfigDto
    {
        public Dictionary<string,string> AllPermissions { get; set; }

        public Dictionary<string, string> GrantedPermissions { get; set; }
        
    }
}