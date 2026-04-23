using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Chat.Dto
{
    public class OnlineUserDto
    {
        public Guid? TenantId { get; set; }

        public Guid UserId { get; set; }

        public string UserName { get; set; } = "";

        public string TenancyName { get; set; } = "";

        public string ConnectionId { get; set; } = "";
    }
}
