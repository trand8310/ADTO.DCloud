using ADTO.DCloud.Authorization;
using ADTO.DCloud.Authorization.Posts;
using ADTO.DCloud.Authorization.Posts.Dto;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.Authorization.Users.Dto;
using ADTO.DCloud.FormScheme;
using ADTO.DCloud.FormScheme.Dto;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.ProjectManage.Dto;
using ADTO.DCloud.WorkFlow.Delegate;
using ADTO.DCloud.WorkFlow.Delegate.Dto;
using ADTO.DCloud.WorkFlow.Delegates.Dto;
using ADTO.DCloud.WorkFlow.Processes;
using ADTO.DCloud.WorkFlow.Processs.Dto;
using ADTO.DCloud.WorkFlow.Schemes.Dto;
using ADTO.DCloud.WorkFlow.StampManage;
using ADTO.DCloud.WorkFlow.Stamps.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Runtime.Session;
using ADTOSharp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Delegates
{
    /// <summary>
    /// 流程委托
    /// </summary>

    //[ADTOSharpAuthorize]
    [ADTOSharpAuthorize(PermissionNames.Pages_Workflow_Delegates)]
    public class DelegateAppservice : DCloudAppServiceBase, IDelegateAppservice
    {
        private readonly IRepository<WorkFlowDelegaterule, Guid> _deletaterulRepository;
        private readonly IRepository<WorkFlowDelegateRelation, Guid> _relationRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private object module;

        public DelegateAppservice(IRepository<WorkFlowDelegaterule, Guid> deletaterulRepository,
            IRepository<WorkFlowDelegateRelation, Guid> relationRepository,
            IRepository<User, Guid> userRepository)
        {
            _deletaterulRepository = deletaterulRepository;
            _relationRepository = relationRepository;
            _userRepository = userRepository;
        }
        /// <summary>
        /// 获取我的委托列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<WorkFlowDelegateruleDto>> GetPageListAsync(GetPageListInput input)
        {
            if(ADTOSharpSession.UserId==null)
            {
                throw new UserFriendlyException("请登录");
            }
            var query = _deletaterulRepository.GetAll()
                .Where(query=>query.CreatorUserId== ADTOSharpSession.GetUserId())
                .WhereIf(!string.IsNullOrEmpty(input.ToUserName), query => query.ToUserName.Contains(input.ToUserName))
                .WhereIf(input.IsActive.HasValue,query=>query.Equals(input.IsActive))
                .WhereIf(input.Type.HasValue,query=>query.Equals(input.Type));
            var totalCount = await query.CountAsync();
            var list = await query.PageBy(input).ToListAsync();
            var result = list.Select(item =>
            {
                var rule = ObjectMapper.Map<WorkFlowDelegateruleDto>(item);
                return rule;
            }).ToList();
            return new PagedResultDto<WorkFlowDelegateruleDto>(totalCount, result);
        }

        /// <summary>
        /// 获取我的委托人（发起委托）
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="code">流程模板编码</param>
        /// <returns></returns>
        public async Task<IEnumerable<WorkFlowDelegateruleDto>> GetMyUserListByCode(GetMyUserListInput input)
        {
            DateTime datetime = DateTime.Now;
            var queryList = from rule in _deletaterulRepository.GetAll()
                            join f in _relationRepository.GetAll() on rule.Id equals f.DelegateRuleId into relations
                            from relation in relations.DefaultIfEmpty()
                            select new { rule, relation };
            queryList = queryList.Where(q => q.rule.ToUserId == input.UserId && q.rule.Type == 1 && q.relation.SchemeInfoCode == input.Code)
                .Where(q => q.rule.BeginDate <= datetime && q.rule.EndDate >= datetime)
                .Where(q => q.rule.IsActive == true);//去掉关闭状态的委托流程
            var list = queryList.ToList().Select(item =>
            {
                var dto = ObjectMapper.Map<WorkFlowDelegateruleDto>(item.rule);
                return dto;
            }).ToList();
            return list;
        }

        /// <summary>
        /// 获取我的委托人（发起委托）-workflow/delegate/users
        /// </summary>
        /// <returns></returns>
        public async Task<List<UserDto>> GetMyUserList(string code)
        {
            var list = new List<Guid>();
            var userInfo = ADTOSharpSession.GetUserId();
            var listTmp = await this.GetMyUserListByCode(new GetMyUserListInput() { UserId = userInfo, Code = code });
            var dic = new Dictionary<Guid, bool>();
            foreach (var item in listTmp)
            {
                if (!dic.ContainsKey(item.CreatorUserId.Value))
                {
                    dic.Add(item.CreatorUserId.Value, true);
                    list.Add(item.CreatorUserId.Value);
                }
            }
            if (list.Count > 0)
            {
                var users = await _userRepository.GetAll().Where(q => q.IsActive == true && list.Contains(q.Id)).ToListAsync();
                return ObjectMapper.Map<List<UserDto>>(users); ;
            }
            else
            {
                return new List<UserDto>();
            }
        }
        /// <summary>
        /// 获取关联的模板数据
        /// </summary>
        /// <returns></returns>
        [HiddenApi]
        public async Task<IEnumerable<WorkFlowDelegateRelation>> GetRelationList(Guid id)
        {
            var list = await _relationRepository.GetAll().Where(t => t.DelegateRuleId == id).ToListAsync();
            return ObjectMapper.Map<List<WorkFlowDelegateRelation>>(list);
        }
        /// <summary>
        /// 根据委托人获取委托记录
        /// </summary>
        /// <param name="type">0 审批委托 1发起委托</param>
        /// <returns></returns>
        [HiddenApi]
        public async Task<IEnumerable<WorkFlowDelegateruleDto>> GetByToUserIdList(int type)
        {
            Guid userId = ADTOSharpSession.GetUserId();
            DateTime datetime = DateTime.Now;
            var list = await _deletaterulRepository.GetAll().Where(t => t.IsActive == true && t.ToUserId == userId && t.BeginDate <= datetime && t.EndDate >= datetime && t.Type == type).ToListAsync();
            return ObjectMapper.Map<List<WorkFlowDelegateruleDto>>(list);
        }

        /// <summary>
        /// 新增委托-workflow/delegate
        /// </summary>
        /// <param name="dto">提交参数</param>
        /// <returns></returns>
        [ADTOSharpAuthorize(PermissionNames.Pages_Workflow_Delegates_Create)]
        public async Task<WorkFlowDelegateruleDto> CreateAsync(CreateWrokFlowDelegateInput input)
        {
            var dto = ObjectMapper.Map<WorkFlowDelegaterule>(input.DelegateRule);
            var id = await _deletaterulRepository.InsertAndGetIdAsync(dto);

            foreach (string schemeInfoCode in input.SchemeInfoList)
            {
                WorkFlowDelegateRelation wfDelegateRuleRelation = new WorkFlowDelegateRelation();
                wfDelegateRuleRelation.DelegateRuleId = id;
                wfDelegateRuleRelation.SchemeInfoCode = schemeInfoCode;
                await _relationRepository.InsertAsync(wfDelegateRuleRelation);
            }

            return ObjectMapper.Map<WorkFlowDelegateruleDto>(dto);
        }
        /// <summary>
        /// 修改委托-workflow/delegate/{}
        /// </summary>
        /// <param name="dto">提交参数</param>
        /// <returns></returns>
        [ADTOSharpAuthorize(PermissionNames.Pages_Workflow_Delegates_Edit)]
        public async Task<WorkFlowDelegateruleDto> UpdateAsync(UpdateWrokFlowDelegateInput input)
        {
            var rule = await _deletaterulRepository.GetAsync(input.DelegateRule.Id);
            ObjectMapper.Map(input.DelegateRule, rule);

            await _relationRepository.DeleteAsync(t => t.DelegateRuleId == input.DelegateRule.Id && !input.SchemeInfoList.Any(a => a.Equals(t.SchemeInfoCode)));
            await _deletaterulRepository.UpdateAsync(rule);

            var relations = await _relationRepository.GetAll().Where(w => w.DelegateRuleId == rule.Id).ToListAsync();

            foreach (string schemeInfoId in input.SchemeInfoList.Where(w => !relations.Any(a => a.DelegateRuleId.Equals(w))))
            {
                WorkFlowDelegateRelation wfDelegateRuleRelation = new WorkFlowDelegateRelation();
                wfDelegateRuleRelation.DelegateRuleId = input.DelegateRule.Id;
                wfDelegateRuleRelation.SchemeInfoCode = schemeInfoId;
                await _relationRepository.InsertAsync(wfDelegateRuleRelation);
            }

            return ObjectMapper.Map<WorkFlowDelegateruleDto>(rule);
        }
        /// <summary>
        /// 根据主键获取委托信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<WorkFlowDelegateruleDto> GetAsync(EntityDto<Guid> input)
        {
            var delegaterule = await _deletaterulRepository.GetAsync(input.Id);
            var dto = ObjectMapper.Map<WorkFlowDelegateruleDto>(delegaterule);
            var list = await _relationRepository.GetAll().Where(t => t.DelegateRuleId.Equals(delegaterule.Id)).Select(d=>d.SchemeInfoCode).ToListAsync();
            dto.SchemeInfoList = list;
            return dto;
        }
        #region 提交数据
        /// <summary>
        /// 删除委托
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [ADTOSharpAuthorize(PermissionNames.Pages_Workflow_Delegates_Delete)]
        public async Task DeleteAsync(EntityDto<Guid> input)
        {
            await _deletaterulRepository.DeleteAsync(input.Id);
        }
        #endregion
    }
}

