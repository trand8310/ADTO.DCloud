using System.Collections.Generic;
using ADTOSharp.Application.Navigation;

namespace ADTOSharp.Web.Models.ADTOSharpUserConfiguration
{
    public class ADTOSharpUserNavConfigDto
    {
        public Dictionary<string, UserMenu> Menus { get; set; }
    }
}