using ADTO.DCloud.Attendances;
using ADTO.DCloud.Authorization;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.EnumManager;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.News.Dto;
using ADTO.DCloud.Notifications;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Notifications;
using ADTOSharp.Organizations;
using ADTOSharp.Runtime.Session;
using ADTOSharp.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.News
{
    /// <summary>
    /// 新闻资讯
    /// </summary>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Information)]
    public class NewsAppService : DCloudAppServiceBase, INewsAppService
    {
        private readonly IRepository<NewsEntity, Guid> _repository;
        private readonly IRepository<NewsViewLog, Guid> _viewLogrepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IRepository<OrganizationUnit, Guid> _orgRepository;
        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
        private readonly IAppNotifier _appNotifier;
        public NewsAppService(IRepository<NewsEntity, Guid> repository,
            IRepository<NewsViewLog, Guid> viewLogrepository,
            IRepository<User, Guid> userRepository,
            IRepository<OrganizationUnit, Guid> orgRepository,
        INotificationSubscriptionManager notificationSubscriptionManager,
        IAppNotifier appNotifier)
        {
            _repository = repository;
            _viewLogrepository = viewLogrepository;
            _userRepository = userRepository;
            _orgRepository = orgRepository;
            _notificationSubscriptionManager = notificationSubscriptionManager;
            _appNotifier = appNotifier;
        }
        /// <summary>
        /// 获取分页-新闻资讯
        /// </summary>
        /// <param name="input">查询实体</param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<PagedResultDto<NewsDto>> GetNewsPagedList(PagedNewsResultRequestDto input)
        {
            var query = this._repository.GetAll().Join(this._userRepository.GetAll(), news => news.CreatorUserId, user => user.Id, (news, user) => new { news, user })
                .Where(q => q.news.TypeId.Equals((int)EnumNewsType.News))
              .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), q => q.news.FullHead.Contains(input.Filter) || q.news.Keyword.Contains(input.Filter));

            //获取总数
            var resultCount = await query.CountAsync();
            var list = query.OrderByDescending(x => x.news.CreationTime).PageBy(input).ToList();
            var ResultList = list.Select(item =>
            {
                var dto = ObjectMapper.Map<NewsDto>(item.news);
                dto.CreateUserName = item.user != null ? item.user.Name : "";
                if (dto.CreationTime <= Convert.ToDateTime("2022-11-07"))
                    dto.NewsContent = CommonHelper.ReplaceHtml(dto.NewsContent, CommonHelper.ReplaceOptions.All);
                return dto;
            }).ToList();

            return new PagedResultDto<NewsDto>(resultCount, ResultList);
        }

        /// <summary>
        /// 获取分页-通知公告
        /// </summary>
        /// <param name="input">查询实体</param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<PagedResultDto<NewsDto>> GetNoticePagedList(PagedNewsResultRequestDto input)
        {
            var query = this._repository.GetAll().Join(this._userRepository.GetAll(), news => news.CreatorUserId, user => user.Id, (news, user) => new { news, user })
                .Where(q => q.news.TypeId.Equals((int)EnumNewsType.Notice))
              .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), q => q.news.FullHead.Contains(input.Filter) || q.news.Keyword.Contains(input.Filter));

            //获取总数
            var resultCount = await query.CountAsync();
            var taskList = query.OrderByDescending(x => x.news.CreationTime).PageBy(input).ToList();
            var ResultList = taskList.ToList().Select(item =>
            {
                var dto = ObjectMapper.Map<NewsDto>(item.news);
                dto.CreateUserName = item.user != null ? item.user.Name : "";
                if (dto.CreationTime <= Convert.ToDateTime("2022-11-07"))
                    dto.NewsContent = System.Web.HttpUtility.HtmlDecode(dto.NewsContent);
                return dto;
            }).ToList();

            return new PagedResultDto<NewsDto>(resultCount, ResultList);
        }

        /// <summary>
        /// 获取分页-钢材行情
        /// </summary>
        /// <param name="input">查询实体</param>
        /// <returns></returns>
        public async Task<PagedResultDto<NewsDto>> GetSteelMarketPagedList(PagedNewsResultRequestDto input)
        {
            var query = this._repository.GetAll()
                .Where(q => q.TypeId.Equals((int)EnumNewsType.SteelMarket))
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), q => q.FullHead.Contains(input.Filter) || q.Keyword.Contains(input.Filter));

            //获取总数
            var resultCount = await query.CountAsync();
            var taskList = query.OrderByDescending(x => x.CreationTime).PageBy(input).ToList();
            var ResultList = taskList.ToList().Select(item =>
            {
                var dto = ObjectMapper.Map<NewsDto>(item);
                return dto;
            }).ToList();

            return new PagedResultDto<NewsDto>(resultCount, ResultList);
        }

        /// <summary>
        /// 根据编号查询对应实体
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<NewsDto> GetAsync(EntityDto<Guid> input)
        {
            var entity = _repository.Get(input.Id);
            var dto = ObjectMapper.Map<NewsDto>(entity);
            if (dto.CreationTime <= Convert.ToDateTime("2022-11-07"))
                dto.NewsContent = CommonHelper.ReplaceHtml(dto.NewsContent, CommonHelper.ReplaceOptions.All);
            return dto;
        }
        #region 提交
        /// <summary>
        /// 新增新增资讯信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>

        [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Information_News_Create)]
        public async Task CreateNewsAsync(CreateNewsDto input)
        {
            NewsEntity entity = ObjectMapper.Map<NewsEntity>(input);
            entity.TypeId = (int)EnumNewsType.News;
            entity.IsActive = true;
            Guid Id = await _repository.InsertAndGetIdAsync(entity);
            if (Id == Guid.Empty)
            {
                throw new UserFriendlyException(L("NewsDoesNotExist"));
            }

            //Notifications
            //订阅消息
            //await _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(user.ToUserIdentifier());
            await _appNotifier.SendMessageAsync(AppNotificationNames.NewsNotification, null);
        }
        /// <summary>
        /// 修改新增资讯信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Information_News_Edit)]
        public async Task UpdateNewsAsync(NewsDto input)
        {
            var entity = _repository.Get(input.Id);
            entity.TypeId = (int)EnumNewsType.News;
            entity.IsActive = true;
            ObjectMapper.Map(input, entity);
            await _repository.UpdateAsync(entity);

            //Notifications
            //订阅消息
            //await _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(user.ToUserIdentifier());
            await _appNotifier.SendMessageAsync(AppNotificationNames.NewsNotification, null);
        }
        /// <summary>
        /// 新增-通知公告信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>

        [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Information_Notice_Create)]
        public async Task CreateNoticeAsync(CreateNewsDto input)
        {
            NewsEntity entity = ObjectMapper.Map<NewsEntity>(input);
            entity.TypeId = (int)EnumNewsType.Notice;
            entity.IsActive = true;
            Guid Id = await _repository.InsertAndGetIdAsync(entity);
            if (Id == Guid.Empty)
            {
                throw new UserFriendlyException(L("NewsDoesNotExist"));
            }
            //Notifications
            //订阅消息
            //await _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(user.ToUserIdentifier());
            await _appNotifier.SendMessageAsync(AppNotificationNames.NoticeNotification, null);
        }
        /// <summary>
        /// 修改-通知公告信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Information_Notice_Edit)]
        public async Task UpdateNoticeAsync(NewsDto input)
        {
            var entity = _repository.Get(input.Id);
            ObjectMapper.Map(input, entity);
            entity.TypeId = (int)EnumNewsType.Notice;
            entity.IsActive = true;
            await _repository.UpdateAsync(entity);
            //Notifications
            //订阅消息
            //await _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(user.ToUserIdentifier());
            await _appNotifier.SendMessageAsync(AppNotificationNames.NoticeNotification, null);
        }

        /// <summary>
        /// 新增-钢材行情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>

        [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Information_SteelMarket_Create)]
        public async Task CreateSteelMarketAsync(CreateNewsDto input)
        {
            NewsEntity entity = ObjectMapper.Map<NewsEntity>(input);
            entity.TypeId = (int)EnumNewsType.SteelMarket;
            entity.IsActive = true;
            Guid Id = await _repository.InsertAndGetIdAsync(entity);
            if (Id == Guid.Empty)
            {
                throw new UserFriendlyException(L("NewsDoesNotExist"));
            }
        }

        /// <summary>
        /// 新增-钢材行情,采集录入
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>

        public async Task CreateSteelMarketAnonymous(CreateNewsDto input)
        {
            NewsEntity entity = ObjectMapper.Map<NewsEntity>(input);
            entity.TypeId = (int)EnumNewsType.SteelMarket;
            entity.IsActive = true;
            Guid Id = await _repository.InsertAndGetIdAsync(entity);
            if (Id == Guid.Empty)
            {
                throw new UserFriendlyException(L("NewsDoesNotExist"));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceAddress"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<NewsDto> GetSteelMarketBySourceAddressWithAnonymous(string sourceAddress)
        {
            var entity = await _repository.FirstOrDefaultAsync(e => e.SourceAddress.Equals(sourceAddress));
            if (entity != null)
            {
                return ObjectMapper.Map<NewsDto>(entity);
            }
            throw new UserFriendlyException(L("NewsDoesNotExist"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceAddress"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<bool> HasSteelMarketBySourceAddressWithAnonymous(string sourceAddress)
        {
            var count = await _repository.CountAsync(e => e.SourceAddress.Equals(sourceAddress));
            return count > 0;
        }


        /// <summary>
        /// 修改-钢材行情-状态
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Information_SteelMarket_Edit)]
        public async Task UpdateSteelMarketAsync(NewsDto input)
        {
            var entity = _repository.Get(input.Id);
            ObjectMapper.Map(input, entity);
            entity.TypeId = (int)EnumNewsType.SteelMarket;
            entity.IsActive = true;
            await _repository.UpdateAsync(entity);
        }
        #endregion

        #region 删除 
        /// <summary>
        /// 删除新增资讯信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// 
        //public async Task DeleteAsync(EntityDto<Guid> input)
        //{
        //    var entity = _repository.Get(input.Id);
        //    if (entity == null || entity.Id == Guid.Empty || entity.TypeId != (int)EnumNewsType.News)
        //    {
        //        throw new UserFriendlyException(L("NewsDoesNotExist"));
        //    }
        //    await _repository.DeleteAsync(entity);
        //}
        /// <summary>
        /// 多删除-删除新闻
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        private async Task DeleteAllAsync(List<Guid> ids)
        {
            foreach (var item in ids)
            {
                var entity = _repository.Get(item);
                if (entity == null || entity.Id == Guid.Empty)
                {
                    throw new UserFriendlyException(L("NewsDoesNotExist"));
                }
                if (entity.TypeId == (int)EnumNewsType.News && await IsGrantedAsync(PermissionNames.Pages_Administration_Information_News_Delete))
                {
                    await _repository.DeleteAsync(entity);
                }
                else if (entity.TypeId == (int)EnumNewsType.Notice && await IsGrantedAsync(PermissionNames.Pages_Administration_Information_Notice_Delete))
                {
                    await _repository.DeleteAsync(entity);
                }
                else if (entity.TypeId == (int)EnumNewsType.SteelMarket && await IsGrantedAsync(PermissionNames.Pages_Administration_Information_SteelMarket_Delete))
                {
                    await _repository.DeleteAsync(entity);
                }
            }
        }
        /// <summary>
        /// 删除新增资讯信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// 
        [ADTOSharpAuthorize("Pages.Administration.Information.News.Delete")]
        private async Task<bool> DeleteNewsAsync(EntityDto<Guid> input)
        {
            var entity = _repository.Get(input.Id);
            if (entity == null || entity.Id != Guid.Empty)
                return false;
            if (entity.TypeId != (int)EnumNewsType.News)
                return false;
            await _repository.DeleteAsync(entity);
            return true;
        }
        /// <summary>
        /// 多删除-删除新闻
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task DeleteAsync([FromBody] DeleteInput input)
        {
            await this.DeleteAllAsync(input.ids);
        }
        #endregion

        #region 新闻浏览记录
        /// <summary>
        /// 浏览记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<NewsViewLogDto>> GetViewLogPagedList(GetViewLogPagedInput input)
        {
            var query = from log in this._viewLogrepository.GetAll()
                        join user in _userRepository.GetAll() on log.CreatorUserId equals user.Id into user
                        from users in user.DefaultIfEmpty()
                        join depart in _orgRepository.GetAll() on users.DepartmentId equals depart.Id into depart
                        from department in depart.DefaultIfEmpty()
                        join com in _orgRepository.GetAll() on users.CompanyId equals com.Id into com
                        from company in com.DefaultIfEmpty()
                        select new { users, log, department, company }
                   ;
            query = query.Where(q => q.log.NewsId.Equals(input.NewsId))
                .WhereIf(!string.IsNullOrWhiteSpace(input.keyword), d => d.users.Name.Contains(input.keyword) || d.users.UserName.Contains(input.keyword));
            //获取总数
            var resultCount = await query.CountAsync();
            var taskList = query.OrderByDescending(x => x.log.CreationTime).PageBy(input).ToList();
            var ResultList = taskList.ToList().Select(item =>
            {
                var dto = ObjectMapper.Map<NewsViewLogDto>(item.log);
                dto.DepartmentName = item.department?.DisplayName;
                dto.CompanyName = item.company?.DisplayName;
                dto.Name = item.users.Name;
                return dto;
            }).ToList();

            return new PagedResultDto<NewsViewLogDto>(resultCount, ResultList);

        }
        /// <summary>
        /// 新增新闻浏览记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task InsertViewLogAsync(EntityDto<Guid> input)
        {
            var entity = _repository.Get(input.Id);
            if (entity.Id == Guid.Empty)
            {
                throw new UserFriendlyException(L("NewsDoesNotExist"));
            }
            entity.PV += 1;
            await _repository.UpdateAsync(entity);

            var currentUser = await _userRepository.GetAsync(ADTOSharpSession.GetUserId());
            await _viewLogrepository.InsertAsync(new NewsViewLog() { NewsId = entity.Id, UserName = (currentUser != null && currentUser.Id != Guid.Empty ? currentUser.UserName : "") });
        }

        #endregion
    }
}
