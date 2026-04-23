using ADTO.DCloud.Modules.Dto;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace ADTO.DCloud.Sessions.Dto
{
    public class UserNavConfigDto
    {

        public List<ModuleDto> Menus { get; set; }
        public List<JObject> Routes { get; set; }
    }
}
