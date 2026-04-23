using ADTO.DCloud.Massages.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Massages
{
    public interface IMessageAppService
    {

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="code">编码</param>
        /// <param name="userIdList">用户列表</param>
        /// <param name="content">消息内容</param>
        /// <param name="messageType">消息类型1.短信 2.邮箱 3.微信 4.IM（站内消息）</param>
        /// <param name="contentId">消息内容id</param>
        public Task SendMsg(SendMassageDto input);

    }
}
