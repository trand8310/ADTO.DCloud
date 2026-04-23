using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Massages.Dto
{
    public class SendMassageDto
    {
        public string Code { get; set; }
        public IEnumerable<Guid> UserIdList { get; set; }
        public string Content { get; set; }
        public string MessageType { get; set; }
        public string ContentId { get; set; } = "";
    }
}
