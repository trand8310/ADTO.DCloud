using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Chat.Dto
{
    public class SystemMessageDto
    {
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public string Level { get; set; } = "info";
        public DateTime CreationTime { get; set; }
    }
}
