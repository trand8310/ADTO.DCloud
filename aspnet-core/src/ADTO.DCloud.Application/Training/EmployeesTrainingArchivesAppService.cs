using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.DataAuthorizes;
using ADTO.DCloud.DataItem;
using ADTO.DCloud.Dto;
using ADTO.DCloud.EmployeeManager;
using ADTO.DCloud.Training.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.EntityFrameworkCore.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Organizations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ADTO.DCloud.Infrastructur;

namespace ADTO.DCloud.Training
{
    /// <summary>
    /// 员工培训记录管理
    /// </summary>
    //[ADTOSharpAuthorize(PermissionNames.Pages_TrainingTrainings)]
    public class EmployeesTrainingArchivesAppService : DCloudAppServiceBase, IEmployeesTrainingArchivesAppService
    {
        #region
        private readonly IRepository<EmployeesTrainingArchives, Guid> _employeesTrainingArchivesRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IRepository<EmployeeInfo, Guid> _employeeInfoRepository;
        private readonly IRepository<OrganizationUnit, Guid> _orgRepository;
        private readonly IRepository<DataItemDetail, Guid> _dataItemDetailRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DataFilterService _dataAuthorization;

        public EmployeesTrainingArchivesAppService(
            IRepository<EmployeesTrainingArchives, Guid> employeesTrainingArchivesRepository,
            IRepository<OrganizationUnit, Guid> orgRepository,
            IRepository<User, Guid> userRepository,
            IRepository<EmployeeInfo, Guid> employeeInfoRepository,
            IRepository<DataItemDetail, Guid> dataItemDetailRepository,
            DataFilterService dataAuthorization,
            IHttpContextAccessor httpContextAccessor)
        {
            _employeesTrainingArchivesRepository = employeesTrainingArchivesRepository;
            _orgRepository = orgRepository;
            _userRepository = userRepository;
            _employeeInfoRepository = employeeInfoRepository;
            _dataItemDetailRepository = dataItemDetailRepository;
            _dataAuthorization = dataAuthorization;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion
        private string GetRequestPath()
        {
            return _httpContextAccessor.HttpContext.Request.Path;
        }
        /// <summary>
        /// 获取员工培训记录
        /// </summary>
        /// <param name="input">查询实体</param>
        /// <returns></returns>
        /// 
        [ADTOSharpAllowAnonymous]
        [DataAuthPermission("培训记录查询")]
        public async Task<PagedResultDto<EmployeesTrainingArchivesDto>> GetTrainingArchivesPagedList(PagedEmployeesTrainingArchivesRequestDto input)
        {
            //当前登录用户
            if (!ADTOSharpSession.UserId.HasValue)
            {
                return new PagedResultDto<EmployeesTrainingArchivesDto>();
            }

            var query = from r in this._employeesTrainingArchivesRepository.GetAllIncluding(p => p.User)
                        join c in this._orgRepository.GetAll() on r.User.CompanyId equals c.Id
                        join d in this._orgRepository.GetAll() on r.User.DepartmentId equals d.Id
                        join u in this._userRepository.GetAll() on r.CreatorUserId equals u.Id
                        select new EmployeesTrainingArchivesDto
                        {
                            Id = r.Id,
                            UserId = r.UserId,
                            UserName = r.User.UserName,
                            Name = r.User.Name,
                            CompanyId = c.Id,
                            CompanyName = c.DisplayName,
                            DeptName = d.DisplayName,
                            PostLevelId = r.PostLevelId,
                            TrainingDate = (DateTime)r.TrainingDate,
                            TrainingTitle = r.TrainingTitle,
                            TrainingHour = r.TrainingHour,
                            TrainingLevel = r.TrainingLevel,
                            TrainingScore = r.TrainingScore,
                            TrainingMonth = r.TrainingMonth,
                            CreationTime = r.CreationTime,
                            CreatorUserId = r.CreatorUserId,
                            CreatorUserName = u.Name,
                        };

            query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), p => p.UserName == input.Keyword || p.Name == input.Keyword || p.TrainingTitle.Contains(input.Keyword));
            query = query.WhereIf(!string.IsNullOrWhiteSpace(input.TrainingMonth), p => p.TrainingMonth == Convert.ToInt32(input.TrainingMonth));

            //数据权限
            string permissionCode = GetCurrentPermissionCode();
            query = await _dataAuthorization.CreateDataFilteredQuery(query, permissionCode);
            //获取总数
            var resultCount = await query.CountAsync();
            var taskList = query.OrderByDescending(o => o.CreationTime).PageBy(input).ToList();
            var ResultList = ObjectMapper.Map<List<EmployeesTrainingArchivesDto>>(taskList);
            foreach (var item in ResultList)
            {
                //职级
                item.PostLevelText = this._dataItemDetailRepository.GetAllIncluding(p => p.Item).Where(p => p.Item.ItemCode == "PostLevel" && p.ItemValue == item.PostLevelId.ToString()).FirstOrDefault()?.ItemName;
            }
            return new PagedResultDto<EmployeesTrainingArchivesDto>(resultCount, ResultList);
        }


        /// <summary>
        /// 批量导入员工培训记录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [IgnoreAntiforgeryToken]
        //[ADTOSharpAuthorize("Pages.Training.Trainings.Create")]
        public async Task<JsonResultModel> BulkImport()
        {
            try
            {
                //当前登录用户
                if (ADTOSharpSession.UserId == null)
                {
                    return new JsonResultModel() { Message = $"操作失败，当前用户未登录", Success = false };
                }
                var userId = ADTOSharpSession.UserId;
                var httpContext = _httpContextAccessor.HttpContext;
                IFormFile formFile = httpContext.Request.Form.Files.FirstOrDefault();
                if (formFile == null)
                {
                    return new JsonResultModel() { Message = $"操作失败，没有文件上传", Success = false };
                }
                string strExtension = Path.GetExtension(formFile.FileName);
                List<EmployeesTrainingArchivesDto> list = new List<EmployeesTrainingArchivesDto>();
                IWorkbook workbook;
                if (".xls".Equals(strExtension, StringComparison.CurrentCultureIgnoreCase))
                {
                    //xls 格式
                    workbook = new HSSFWorkbook(formFile.OpenReadStream());
                }
                else if (".xlsx".Equals(strExtension, StringComparison.CurrentCultureIgnoreCase))
                {
                    //xlsx 格式
                    workbook = new XSSFWorkbook(formFile.OpenReadStream());
                }
                else
                {
                    workbook = null;
                }

                if (workbook != null)
                {
                    var users = await this._employeeInfoRepository.GetAll().Where(q => q.IsActive == true).ToListAsync();
                    ISheet sheet = workbook.GetSheetAt(0);
                    int rows = sheet.PhysicalNumberOfRows;

                    for (int index = 1; index < rows; index++)
                    {
                        IRow row = sheet.GetRow(index);
                        if (row == null)
                        {
                            continue;
                        }
                        EmployeesTrainingArchivesDto trainingArchivesDto = new EmployeesTrainingArchivesDto();
                        //用户工号
                        trainingArchivesDto.UserName = row.Cells[0].ToString().Trim();
                        trainingArchivesDto.Name = row.Cells[1].ToString().Trim();
                        trainingArchivesDto.DeptName = row.Cells[2].ToString().Trim();
                        trainingArchivesDto.CompanyName = row.Cells[3].ToString().Trim();
                        trainingArchivesDto.TrainingDate = Extensions.ToDate(row.Cells[4]);
                        trainingArchivesDto.TrainingTitle = row.Cells[5].ToString().Trim();
                        trainingArchivesDto.TrainingHour = Extensions.ToDecimal(row.Cells[6]);
                        trainingArchivesDto.TrainingLevel = row.Cells[7].ToString().Trim();
                        trainingArchivesDto.TrainingScore = row.Cells[8].ToString().Trim();
                        trainingArchivesDto.TrainingMonth = Extensions.ToInt(row.Cells[9]);
                        trainingArchivesDto.CreatorUserId = userId;

                        var vUser = users.Where(p => p.UserName == trainingArchivesDto.UserName.TrimEnd().TrimStart()).FirstOrDefault();
                        if (vUser != null)
                        {
                            trainingArchivesDto.UserId = vUser.Id;
                            trainingArchivesDto.PostLevelId = vUser.PostLevelId;
                        }
                        else
                        {
                            return new JsonResultModel() { Message = $"操作失败，第{index}行工号或用户信息有误", Success = false };
                        }
                        
                        var vDepInfo = await _orgRepository.FirstOrDefaultAsync(p => p.DisplayName == trainingArchivesDto.DeptName);
                        if (vDepInfo != null)
                        {
                            trainingArchivesDto.DeptId = vDepInfo.Id;
                        }
                        else
                        {
                            return new JsonResultModel() { Message = $"操作失败，第{index}行部门信息有误", Success = false };
                        }

                        var cInfo = await _orgRepository.FirstOrDefaultAsync(p => p.DisplayName == trainingArchivesDto.CompanyName);
                        if (cInfo != null)
                        {
                            trainingArchivesDto.CompanyId = cInfo.Id;
                        }
                        else
                        {
                            return new JsonResultModel() { Message = $"操作失败，第{index}行公司信息有误", Success = false };
                        }


                        list.Add(trainingArchivesDto);
                    }
                }

                else
                {
                    return new JsonResultModel() { Message = $"操作失败，请检查Excel文件格式", Success = false };
                }

                if (list != null && list.Count > 0)
                {
                    var modelList = ObjectMapper.Map<List<EmployeesTrainingArchives>>(list);
                    await _employeesTrainingArchivesRepository.InsertRangeAsync(modelList);
                    //await this.Repository.GetDbContext().BulkInsertAsync(modelList);
                }
                return new JsonResultModel() { Message = $"操作成功", Success = true };
            }
            catch (Exception ex)
            {
                return new JsonResultModel() { Message = $"操作失败,请联系开发人员，此失败的错误信息为：{ex.Message}。", Success = false };
            }
        }

        /// <summary>
        /// 新增员工培训记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[ADTOSharpAuthorize("Pages.Training.Trainings.Create")]
        public async Task<JsonResultModel> CreateInfo(CreateEmployeesTrainingArchivesDto input)
        {
            var entity = ObjectMapper.Map<EmployeesTrainingArchives>(input);
            var userInfo = await _employeeInfoRepository.FirstOrDefaultAsync(p => p.UserName == input.UserName);
            if (userInfo == null)
            {
                return new JsonResultModel() { Message = $"工号不存在", Success = false };
            }

            entity.User = userInfo.User;
            entity.CompanyId = (Guid)userInfo.CompanyId;
            entity.DeptId = userInfo.DepartmentId;
            entity.PostLevelId = userInfo.PostLevelId;
            entity.TrainingMonth = Convert.ToInt32(input.TrainingDate.ToString("yyyyMM"));
            var Id = await _employeesTrainingArchivesRepository.InsertAndGetIdAsync(entity);
            if (Id == Guid.Empty)
            {
                return new JsonResultModel() { Message = $"操作失败", Success = false };
            }
            return new JsonResultModel() { Message = $"操作成功", Success = true };
        }

        /// <summary>
        /// 修改员工培训记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        //[ADTOSharpAuthorize("Pages.Training.Trainings.Create")]
        public async Task<JsonResultModel> UpdateInfo(EmployeesTrainingArchivesDto input)
        {
            var entity = _employeesTrainingArchivesRepository.Get(input.Id);
            if (entity == null || string.IsNullOrEmpty(entity.Id.ToString()))
            {
                return new JsonResultModel() { Message = $"记录不存在", Success = false };
            }
            var userInfo = await _employeeInfoRepository.FirstOrDefaultAsync(p => p.UserName == input.UserName);
            if (userInfo == null)
            {
                return new JsonResultModel() { Message = $"工号不存在", Success = false };
            }
            entity = ObjectMapper.Map<EmployeesTrainingArchives>(input);

            entity.User = userInfo.User;
            entity.CompanyId = (Guid)userInfo.CompanyId;
            entity.DeptId = userInfo.DepartmentId;
            entity.PostLevelId = userInfo.PostLevelId;
            entity.TrainingMonth = Convert.ToInt32(input.TrainingDate.ToString("yyyyMM"));
            await _employeesTrainingArchivesRepository.UpdateAsync(entity);

            return new JsonResultModel() { Message = $"操作成功", Success = true };
        }

    }
}
