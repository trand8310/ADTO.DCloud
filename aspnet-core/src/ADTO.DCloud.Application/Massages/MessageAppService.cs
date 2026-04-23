using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.Massages.Dto;
using ADTO.DCloud.Messages;
using ADTO.DCloud.WorkFlow.Schemes;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using NUglify.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Massages
{
    /// <summary>
    /// 即时通讯消息表
    /// </summary>
    public class MessageAppService : DCloudAppServiceBase, IMessageAppService
    {
        private readonly IRepository<Message, Guid> _repository;
        public MessageAppService(IRepository<Message, Guid> repository)
        {
            _repository = repository;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="userIdList"></param>
        /// <param name="content"></param>
        /// <param name="messageType"></param>
        /// <param name="contentId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task SendMsg(SendMassageDto input)
        {
            try
            {
                if (!string.IsNullOrEmpty(input.Content) && input.UserIdList != null)
                {
                    foreach (var userId in input.UserIdList)
                    {
                        Message iMMsgEntity = new Message();
                        iMMsgEntity.SendUserId = input.Code;
                        iMMsgEntity.RecvUserId = userId + "";
                        iMMsgEntity.Content = input.Content;
                        iMMsgEntity.ContentId = input.ContentId;
                        iMMsgEntity.IsSystem = 1;
                        iMMsgEntity.IsRead = 0;
                        await _repository.InsertAsync(iMMsgEntity);
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// 删除数据(虚拟删除)
        /// </summary>
        /// <param name="contentId">内容主键</param>
        /// <returns></returns>
        public async Task VirtualDeleteByContentId(string contentId)
        {
            var entity = await _repository.GetAll().Where(t => t.ContentId == contentId && t.RecvUserId == ADTOSharpSession.GetUserId().ToString()).FirstOrDefaultAsync();
            if (entity != null)
                await _repository.DeleteAsync(entity.Id);
        }
    }
}

