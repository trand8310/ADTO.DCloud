using ADTO.DCloud.Authorization;
using ADTO.DCloud.Authorization.Posts;
using ADTO.DCloud.Authorization.Roles;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.DataAuthorizes;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.Modules;
using ADTO.DCloud.Modules.Dto;
using ADTO.DCloud.WorkFlow.Processes;
using ADTO.DCloud.WorkFlow.Processs.Dto;
using ADTO.DCloud.WorkFlow.Schemes.Dto;
using ADTO.DCloud.WorkFlow.Stamps.Dto;
using ADTOSharp;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Runtime.Session;
using ADTOSharp.UI;
using Humanizer;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using Newtonsoft.Json;
using NuGet.Packaging.Signing;
using NuGet.Protocol;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Twilio.TwiML.Voice;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ADTO.DCloud.WorkFlow.Schemes
{
    /// <summary>
    /// 流程管理
    /// </summary>
    [ADTOSharpAuthorize(PermissionNames.Pages_Workflow_FlowDesign)]
    public class WorkFlowSchemeinfoAppService : DCloudAppServiceBase, IWorkFlowSchemeAppService
    {

        private readonly IRepository<WorkFlowSchemeinfo, Guid> _schemeInfoRepository;
        private readonly IRepository<WorkFlowScheme, Guid> _schemeRepository;
        private readonly IRepository<WorkFlowSchemeauth, Guid> _authRepository;
        private readonly IRepository<WorkFlowProcess, Guid> _processRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DataAuthorizesAppService _dataAuthorizesBaseService;

        #region ctor
        public WorkFlowSchemeinfoAppService(IRepository<WorkFlowSchemeinfo, Guid> schemeInfoRepository, IRepository<WorkFlowScheme, Guid> schemeRepository,
             IRepository<WorkFlowSchemeauth, Guid> authRepository,
              IRepository<User, Guid> userRepository,
             IHttpContextAccessor httpContextAccesso,
             IRepository<WorkFlowProcess, Guid> processRepository,
             DataAuthorizesAppService dataAuthorizesBaseService
            )
        {
            _schemeInfoRepository = schemeInfoRepository;
            _schemeRepository = schemeRepository;
            _httpContextAccessor = httpContextAccesso;
            _authRepository = authRepository;
            _userRepository = userRepository;
            _processRepository = processRepository;
            _dataAuthorizesBaseService = dataAuthorizesBaseService;
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 获取文件路径
        /// </summary>
        /// <returns></returns>
        private string GetRequestPath()
        {
            return IocManager.Instance.Resolve<IHttpContextAccessor>().HttpContext.Request.Path;
        }
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[HttpGet("workflow/scheme/page")]
        public async Task<PagedResultDto<WorkFlowSchemeinfoDto>> GetAllAsync(GetSchemeInfoInput input)
        {
            var query = await _schemeInfoRepository.GetAllIncludingAsync(t => t.CreatorUser);
            query = query.WhereIf(!string.IsNullOrWhiteSpace(input.KeyWord), q => q.Name.Contains(input.KeyWord) || q.Code.Contains(input.KeyWord))
              .WhereIf(!string.IsNullOrWhiteSpace(input.Category), q => q.Category.Contains(input.Category));
            query = await _dataAuthorizesBaseService.CreateDataFilteredQuery<WorkFlowSchemeinfo>(query, GetRequestPath());


            if (input.Sorting != null)
                query = query.OrderBy(input.Sorting);

            var totalCount = query.Count();
            var list = await query.PageBy(input).ToListAsync();
            var result = list.Select(item =>
            {
                var scheme = ObjectMapper.Map<WorkFlowSchemeinfoDto>(item);
                scheme.CreateUserName = item.CreatorUser == null ? "" : item.CreatorUser.Name;
                return scheme;
            }).ToList();
            return new PagedResultDto<WorkFlowSchemeinfoDto>(totalCount, result);
        }


        /// <summary>
        /// 根据流程编码获取流程模板数据，对应workflow/scheme/{Code}接口
        /// </summary>
        /// <param name="code">流程编码</param>
        /// <returns></returns>
        //[HttpGet("workflow/scheme/{code}")]
        [ADTOSharpAllowAnonymous]
        public async Task<WFSchemeDto> GetFormData(GetFormDataInput input)
        {
            if (!ADTOSharpSession.UserId.HasValue)
                throw new UserFriendlyException(L("DefaultError403"));

            var scheme = await _schemeInfoRepository.GetAll().Join(_schemeRepository.GetAll(), info => info.SchemeId, scheme => scheme.Id, (info, scheme) => new { info, scheme })
                .Where(q => q.info.Code == input.Code).FirstOrDefaultAsync();
            if (scheme == null)
                return null;
            var authList = await _authRepository.GetAll().Where(q => q.SchemeInfoId.Equals(scheme.info.Id)).ToListAsync();
            WFSchemeDto data = new WFSchemeDto()
            {
                Schemeinfo = ObjectMapper.Map<WorkFlowSchemeinfoDto>(scheme.info),
                Scheme = ObjectMapper.Map<WorkFlowSchemeDto>(scheme.scheme),
                SchemeAuthList = ObjectMapper.Map<List<WorFlowSchemeAuthDto>>(authList)
            };
            return data;
        }

        /// <summary>
        /// 获取流程设计(依流程Id)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<WFSchemeDto> GetAsync(EntityDto<Guid> input)
        {
            var schemeinfo = await _schemeInfoRepository.GetAsync(input.Id);
            var scheme = await _schemeRepository.GetAsync(schemeinfo.SchemeId);
            var authList = await _authRepository.GetAll().Where(q => q.SchemeInfoId.Equals(schemeinfo.Id)).ToListAsync();
            WFSchemeDto data = new WFSchemeDto()
            {
                Schemeinfo = ObjectMapper.Map<WorkFlowSchemeinfoDto>(schemeinfo),
                Scheme = ObjectMapper.Map<WorkFlowSchemeDto>(scheme),
                SchemeAuthList = ObjectMapper.Map<List<WorFlowSchemeAuthDto>>(authList)
            };
            return data;
        }

        /// <summary>
        /// 获取流程列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<WorkFlowSchemeinfoDto>> GetSchemes()
        {
            var list = await _schemeInfoRepository.GetAll().Where(q => q.IsActive == true)
                .ToListAsync();
            return ObjectMapper.Map<List<WorkFlowSchemeinfoDto>>(list);
        }

        /// <summary>
        /// 发起流程-获取自定义流程列表，对应workflow/scheme/mylist接口
        /// </summary>
        /// <returns></returns>
        //[HttpGet("workflow/scheme/mylist")]
        [ADTOSharpAllowAnonymous]
        public async Task<IEnumerable<WorkFlowSchemeinfoDto>> GetMyList(GetMyListInput input)
        {
            if (!ADTOSharpSession.UserId.HasValue)
                throw new UserFriendlyException(L("DefaultError403"));

            var userInfo = await this.GetCurrentUserAsync();
            List<Guid> ids = new List<Guid>();
            //获取所有管理员用户
            var userAdminList = await UserManager.GetUsersInAdminRoleAsync();
            var user = userAdminList.Where(q => q.Id == userInfo.Id);
            //不是管理员的时候根据权限获取数据
            if (user.Count() < 1)
            {
                if (!userInfo.Id.IsEmpty())
                {
                    ids.Add(userInfo.Id);
                }
                var postIds = await UserManager.GetCurrentUserPostIds();
                var roles = await UserManager.GetUserRolesAsync(userInfo);
                var roleIds = roles.Select(d => d.Id).ToList();
                ids.AddRange(postIds);
                ids.AddRange(roleIds);
            }

            var res = await this.GetInfoList(ids);
            res = res.WhereIf(!string.IsNullOrWhiteSpace(input.Category), q => q.Category.Equals(input.Category))
                .WhereIf(!string.IsNullOrWhiteSpace(input.KeyWord), q => q.Name.Contains(input.KeyWord) || q.Code.Contains(input.KeyWord));
            return res;
        }

        /// <summary>
        /// 发起流程-获取自定义流程列表，对应workflow/scheme/mylist接口
        /// </summary>
        /// <returns></returns>
        //[HttpGet("workflow/scheme/mylist")]
        [ADTOSharpAllowAnonymous]
        public async Task<PagedResultDto<WorkFlowSchemeinfoDto>> GetMyPageList(GetMyPageListInput input)
         {
            if (!ADTOSharpSession.UserId.HasValue)
                throw new UserFriendlyException(L("DefaultError403"));
            var userInfo = await this.GetCurrentUserAsync();
            List<Guid> ids = new List<Guid>();
            //获取所有管理员用户
            var userAdminList = await UserManager.GetUsersInAdminRoleAsync();
            var user = userAdminList.Where(q => q.Id == userInfo.Id);
            //不是管理员的时候根据权限获取数据
            if (user.Count() < 1)
            {
                if (!userInfo.Id.IsEmpty())
                {
                    ids.Add(userInfo.Id);
                }
                var postIds = await UserManager.GetCurrentUserPostIds();
                var roles = await UserManager.GetUserRolesAsync(userInfo);
                var roleIds = roles.Select(d => d.Id).ToList();
                ids.AddRange(postIds);
                ids.AddRange(roleIds);
            }
            var schemeInfoIds =await _authRepository.GetAll().Where(t => ids.Contains(t.ObjId.Value) && t.Type != 2 && t.Type != 3).Select(d => d.SchemeInfoId).ToListAsync();
            var query =  _schemeInfoRepository.GetAll().Where(t => t.IsActive == true && t.Mark == 1 && (schemeInfoIds.Contains(t.Id) || t.AuthType == 1));

            query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Category), q => q.Category.Equals(input.Category))
                .WhereIf(!string.IsNullOrWhiteSpace(input.KeyWord), q => q.Name.Contains(input.KeyWord) || q.Code.Contains(input.KeyWord));

            if (!string.IsNullOrWhiteSpace(input.Sorting))
                query = query.OrderBy(input.Sorting);
            var totalCount = query.Count();
            var list = await query.PageBy(input).ToListAsync();
            var listDtos = ObjectMapper.Map<List<WorkFlowSchemeinfoDto>>(list);
            return new PagedResultDto<WorkFlowSchemeinfoDto>(totalCount, listDtos);
        }
        #endregion

        #region 获取批量打印流程列表
        /// <summary>
        /// 获取批量打印流程列表
        /// </summary>
        /// <returns></returns>
        //[HttpGet("workflow/scheme/printlist")]
        public async Task<List<WorkFlowSchemeinfoDto>> GetPrintList()
        {
            var list = await _schemeInfoRepository.GetAll().Where(q => q.IsActive == true || q.AuthType == 1 || q.Mark == 1)
                .ToListAsync();
            return ObjectMapper.Map<List<WorkFlowSchemeinfoDto>>(list);
        }
        #endregion

        #region 获取流程模板历史数据
        /// <summary>
        /// 获取流程模板分页数据-模板历史数据--workflow/scheme/historys/{SchmeInfoId}
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<WorkFlowSchemeDto>> GetHistorySchemePageList(GetHistorySchemePageInput input)
        {
            var query = await _schemeRepository.GetAllIncludingAsync(t => t.CreatorUser);
            query = query.Where(q => q.SchemeInfoId.Equals(input.SchemeInfoId));
            var totalCount = query.Count();
            var list = await query.OrderByDescending(d => d.CreationTime).PageBy(input).ToListAsync();
            var listDtos = list.Select(item =>
            {
                var scheme = ObjectMapper.Map<WorkFlowSchemeDto>(item);
                scheme.CreatorUserName = item.CreatorUser == null ? "" : item.CreatorUser.Name;
                return scheme;
            }).ToList();
            return new PagedResultDto<WorkFlowSchemeDto>(totalCount, listDtos);

        }
        /// <summary>
        /// 获取流程模板数据-模板历史数据--不分页workflow/scheme/history/{SchmeInfoId}
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<WorkFlowSchemeDto>> GetHistorySchemeList(GetHistorySchemeInput input)
        {
            var query = await _schemeRepository.GetAllIncludingAsync(t => t.CreatorUser);
            var list = await query.OrderByDescending(d => d.CreationTime).ToListAsync();
            var result = list.Select(item =>
            {
                var scheme = ObjectMapper.Map<WorkFlowSchemeDto>(item);
                scheme.CreatorUserName = item.CreatorUser == null ? "" : item.CreatorUser.Name;
                return scheme;
            }).ToList();
            return result;
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 新增流程-workflow/scheme
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<WorkFlowSchemeinfoDto> CreateAsync(CreateSchemeInput input)
        {
            //判断流程编码是否存在
            var schemeinfo = _schemeInfoRepository.GetAll().Where(q => q.Code.Equals(input.Schemeinfo.Code));
            if (schemeinfo.Count() > 0)
            {
                throw new UserFriendlyException("流程编码重复");
            }

            var schemeinfoentiy = ObjectMapper.Map<WorkFlowSchemeinfo>(input.Schemeinfo);
            schemeinfoentiy.TenantId = ADTOSharpSession.TenantId;
            var entity = await _schemeInfoRepository.InsertAsync(schemeinfoentiy);

            #region 流程模板

            var scheme = ObjectMapper.Map<WorkFlowScheme>(input.Scheme);
            if (!string.IsNullOrEmpty(input.Scheme.Content))
                scheme.Content = AESHelper.AesDecrypt(input.Scheme.Content, "ADTODCloud");

            scheme.SchemeInfoId = entity.Id;
            WorkFlowScheme workFlowScheme = await _schemeRepository.InsertAsync(scheme);
            entity.SchemeId = workFlowScheme.Id;

            #endregion

            #region 保存流程权限
            var authList = await _authRepository.GetAll().Where(q => q.SchemeInfoId.Equals(entity.Id)).ToListAsync();
            foreach (var item in authList)
            {
                await _authRepository.DeleteAsync(item);
            }
            foreach (var item in input.SchemeAuthList)
            {
                var auth = ObjectMapper.Map<WorkFlowSchemeauth>(item);
                auth.SchemeInfoId = entity.Id;
                await _authRepository.InsertAsync(auth);
            }
            #endregion

            CurrentUnitOfWork.SaveChanges();
            return ObjectMapper.Map<WorkFlowSchemeinfoDto>(schemeinfoentiy);
        }
        /// <summary>
        /// 修改流程workflow/scheme/{Id}
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<WorkFlowSchemeinfoDto> UpdateAsync(UpdateSchemeInput input)
        {
            var schemeinfo = await _schemeInfoRepository.GetAsync(input.Schemeinfo.Id);
            ObjectMapper.Map(input.Schemeinfo, schemeinfo);

            #region 保存流程模板

            var scheme = ObjectMapper.Map<WorkFlowScheme>(input.Scheme);

            if (!string.IsNullOrEmpty(input.Scheme.Content))
                scheme.Content = AESHelper.AesDecrypt(input.Scheme.Content, "ADTODCloud");

            scheme.SchemeInfoId = schemeinfo.Id;
            WorkFlowScheme workFlowScheme = await _schemeRepository.InsertAsync(scheme);

            #endregion

            #region 保存流程权限
            var authList = await _authRepository.GetAll().Where(q => q.SchemeInfoId.Equals(schemeinfo.Id)).ToListAsync();
            foreach (var item in authList)
            {
                await _authRepository.DeleteAsync(item);
            }
            foreach (var item in input.SchemeAuthList)
            {
                var auth = ObjectMapper.Map<WorkFlowSchemeauth>(item);
                auth.SchemeInfoId = schemeinfo.Id;
                await _authRepository.InsertAsync(auth);
            }
            #endregion

            schemeinfo.SchemeId = workFlowScheme.Id;
            await _schemeInfoRepository.UpdateAsync(schemeinfo);

            return ObjectMapper.Map<WorkFlowSchemeinfoDto>(schemeinfo);
        }


        /// <summary>
        /// 删除流程-workflow/scheme/{id}
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteModuleAsync(EntityDto<Guid> input)
        {
            await _schemeInfoRepository.DeleteAsync(input.Id);
            return new JsonResult(new
            {
                Message = "删除成功！",
                Success = true
            });
        }


        /// <summary>
        /// 启用/禁用流程-workflow/scheme/state/{id}/{State}
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="state">状态1启用0禁用</param>
        /// <returns></returns>
        public async Task<IActionResult> UpDateState(UpDateStateInput input)
        {
            var schemeinfo = await _schemeInfoRepository.GetAsync(input.Id);
            schemeinfo.IsActive = input.State;
            await _schemeInfoRepository.UpdateAsync(schemeinfo);
            return new JsonResult(new
            {
                Message = (input.State ? "启用" : "禁用") + "成功！",
                Success = true
            });
        }
        /// <summary>
        /// 更新表单模板版本-workflow/scheme/history/{id}/{schemeId}
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="schemeId">流程模板主键</param>
        /// <returns></returns>
        public async Task<IActionResult> UpdateScheme(UpdateSchemeDto input)
        {
            var schemeInfo = await _schemeInfoRepository.GetAsync(input.Id);
            if (schemeInfo == null || schemeInfo.Id.IsEmpty())
            {
                throw new UserFriendlyException("流程不存在");
            }
            var scheme = await _schemeRepository.GetAsync(input.SchemeId);
            if (scheme == null || scheme.Id.IsEmpty())
            {
                throw new UserFriendlyException("流程模板不存在");
            }
            schemeInfo.SchemeId = scheme.Id;
            var res = _schemeInfoRepository.UpdateAsync(schemeInfo);
            return new JsonResult(new
            {
                Message = "保存成功！",
                Success = true
            });

        }


        #endregion

        #region 导入导出

        /// <summary>
        /// 导出流程模版
        /// </summary>
        /// <param name="keyValue">流程主键</param>
        /// <param name="path">目录</param>
        /// <returns></returns>
        public async Task<IActionResult> ExportScheme(ExportSchemeInput input)
        {
            var infoEntity = await _schemeInfoRepository.GetAsync(input.Id);
            var schemeEntity = await _schemeRepository.GetAsync(infoEntity.SchemeId);
            var authList = await _authRepository.GetAll().Where(q => q.SchemeInfoId.Equals(infoEntity.Id)).ToListAsync();
            schemeEntity.Content = AESHelper.AesEncrypt(schemeEntity.Content, "ADTODCloud");
            WFSchemeDto jsonData = new WFSchemeDto()
            {
                Schemeinfo = ObjectMapper.Map<WorkFlowSchemeinfoDto>(infoEntity),
                Scheme = ObjectMapper.Map<WorkFlowSchemeDto>(schemeEntity),
                SchemeAuthList = ObjectMapper.Map<List<WorFlowSchemeAuthDto>>(authList)
            };
            var fileName = $"{input.Path}/{infoEntity.Name}{infoEntity.Code}.json";
            return new FileContentResult(System.Text.Encoding.UTF8.GetBytes(jsonData.ToJson()), "application/json")
            {
                FileDownloadName = fileName  // 设置下载文件名
            };
        }

        /// <summary>
        /// 导入流程模板数据
        /// </summary>
        /// <param name="jsonData"></param>IFormFile file
        [HttpPost]
        public async Task<IActionResult> ImportScheme()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            IFormFile file = httpContext.Request.Form.Files.FirstOrDefault();
            if (file == null || file.Length == 0)
            {
                return new JsonResult(new
                {
                    Message = $"操作失败，没有文件上传"
                });
            }
            // 读取文件内容
            using (var stream = file.OpenReadStream())
            using (var reader = new StreamReader(stream))
            {
                var jsonContent = await reader.ReadToEndAsync();
                // 反序列化为 WFSchemeDto 对象
                var schemes = JsonConvert.DeserializeObject<WFSchemeDto>(jsonContent);
                //根据流程编码查询流程模板
                var schemeInfo = await _schemeInfoRepository.GetAll().Where(q => q.Code.Equals(schemes.Schemeinfo.Code)).FirstOrDefaultAsync();
                //如果存在流程模板，则修改反之新增
                if (schemeInfo != null && schemeInfo.Id != Guid.Empty)
                {
                    schemes.Schemeinfo.Id = schemeInfo.Id;
                    ObjectMapper.Map(schemes.Schemeinfo, schemeInfo);
                    WorkFlowScheme workFlowScheme = await _schemeRepository.InsertAsync(new WorkFlowScheme() { TenantId = ADTOSharpSession.TenantId, Type = schemes.Scheme.Type, SchemeInfoId = schemeInfo.Id, Content = schemes.Scheme.Content });
                    schemeInfo.SchemeId = workFlowScheme.Id;
                    await _schemeInfoRepository.UpdateAsync(schemeInfo);
                }
                else
                {
                    var scheme = ObjectMapper.Map<WorkFlowSchemeinfo>(schemes.Schemeinfo);
                    scheme.Id = Guid.Empty;
                    scheme.TenantId = ADTOSharpSession.TenantId;
                    var entity = await _schemeInfoRepository.InsertAsync(scheme);
                    WorkFlowScheme workFlowScheme = await _schemeRepository.InsertAsync(new WorkFlowScheme() { TenantId = ADTOSharpSession.TenantId, Type = schemes.Scheme.Type, SchemeInfoId = entity.Id, Content = schemes.Scheme.Content });
                    entity.SchemeId = workFlowScheme.Id;
                }
            }
            return new JsonResult(new
            {
                Message = $"导入成功",
                Success = true
            });
        }
        #endregion


        #region 获取权限相关的用户信息 
        /// <summary>
        /// 获取自定义流程列表
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="postIds">岗位id</param>
        /// <param name="roleIds">角色id</param>
        /// <returns></returns>
        private async Task<IEnumerable<WorkFlowSchemeinfoDto>> GetInfoList(List<Guid> ids)
        {
            var list = _authRepository.GetAll().Where(t => ids.Contains(t.ObjId.Value) && t.Type != 2 && t.Type != 3);
            var schemeinfoIds = await list.Select(d => d.SchemeInfoId).ToListAsync();
            var schemeInfos = await _schemeInfoRepository.GetAll().Where(t => t.IsActive == true && t.Mark == 1 && (schemeinfoIds.Contains(t.Id) || t.AuthType == 1)).ToListAsync();
            return ObjectMapper.Map<List<WorkFlowSchemeinfoDto>>(schemeInfos);
        }
        /// <summary>
        /// 获取模板基础信息的实体
        /// </summary>
        /// <param name="code">流程编号</param>
        /// <returns></returns>
        public async Task<WorkFlowSchemeinfoDto> GetSchemeInfoEntityByCode(string code)
        {
            var schemeInfo = await _schemeInfoRepository.GetAll().Where(q => q.Code.Equals(code)).FirstOrDefaultAsync();
            return ObjectMapper.Map<WorkFlowSchemeinfoDto>(schemeInfo);
        }
        /// <summary>
        /// 获取模板基础信息的实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task<WorkFlowSchemeinfoDto> GetSchemeInfoEntity(Guid keyValue)
        {
            var schemeInfo = await _schemeInfoRepository.GetAsync(keyValue);
            return ObjectMapper.Map<WorkFlowSchemeinfoDto>(schemeInfo);
        }
        /// <summary>
        /// 根据主键获取模板
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task<WorkFlowSchemeDto> GetSchemeEntity(Guid keyValue)
        {
            var schemeInfo = await _schemeRepository.GetAsync(keyValue);
            return ObjectMapper.Map<WorkFlowSchemeDto>(schemeInfo);
        }

        #endregion
    }
}

