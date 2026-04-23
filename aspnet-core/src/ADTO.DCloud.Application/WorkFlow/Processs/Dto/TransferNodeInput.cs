using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Processs.Dto
{
    public class TransferNodeInput:EntityDto<Guid>
    {
        /// <summary>
        /// 开始Id
        /// </summary>
        public Guid StartId { get; set; }
        /// <summary>
        /// 结束Id
        /// </summary>
        public Guid EndId { get; set; }
    }
}
