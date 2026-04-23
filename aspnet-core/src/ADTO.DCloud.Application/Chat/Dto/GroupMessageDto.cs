using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Chat.Dto
{
    public class GroupMessageDto
    {
        public string GroupName { get; set; } = "";
        public string Message { get; set; } = "";
        public string SenderUserName { get; set; } = "";
        public Guid? SenderUserId { get; set; }
        public Guid? SenderTenantId { get; set; }
        public DateTime CreationTime { get; set; }
    }

}
