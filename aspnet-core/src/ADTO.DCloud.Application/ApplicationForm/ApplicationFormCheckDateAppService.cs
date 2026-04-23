using ADTO.DCloud.ApplicationForm.Dto;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.Configuration;
using ADTO.DCloud.Infrastructur;
using ADTOSharp;
using ADTOSharp.Authorization;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;


namespace ADTO.DCloud.ApplicationForm
{
    /// <summary>
    /// 
    /// </summary>
    [ADTOSharpAuthorize]
    public class ApplicationFormCheckDateAppService : DCloudAppServiceBase, IApplicationFormCheckDateAppService
    {
        private readonly IRepository<ApplicationFormCheckDate> _repository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfigurationRoot _appConfiguration;
        private readonly IRepository<Adto_Abs, Guid> _absrepository;
        private readonly IRepository<Adto_Onb, Guid> _onbrepository;
        private readonly IRepository<Adto_Out, Guid> _outrepository;
        private readonly IRepository<DisableUserWrokFlow, Guid> _disablerepository;
        public ApplicationFormCheckDateAppService(IRepository<ApplicationFormCheckDate> repository,
              IRepository<User, Guid> userRepository,
              IWebHostEnvironment webHostEnvironment,
              IRepository<Adto_Onb, Guid> onbrepository,
              IRepository<Adto_Out, Guid> outrepository,
              IRepository<Adto_Abs, Guid> absrepository,
              IRepository<DisableUserWrokFlow, Guid> disablerepository)
        {
            _repository = repository;
            _userRepository = userRepository;
            _webHostEnvironment = webHostEnvironment;
            _appConfiguration = AppConfigurations.Get(_webHostEnvironment.ContentRootPath);
            _absrepository = absrepository;
            _onbrepository = onbrepository;
            _outrepository = outrepository;
            _disablerepository = disablerepository;
        }
        /// <summary>
        /// 获取指定用户是否禁用指定申请单
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="resourceTable"></param>
        /// <returns></returns>
        public async Task<List<DisableUserWrokFlowDto>> GetDisableUserListByUserId(Guid userId, string resourceTable)
        {
            var list = await this._disablerepository.GetAll().Where(p =>  p.ResourceTable == resourceTable).ToListAsync();
            return ObjectMapper.Map<List<DisableUserWrokFlowDto>>(list.ToList());
        }
        /// <summary>
        /// 是否存在时间段内的申请单（请假、外出、出差）
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task IsExistApplication(Guid userId, DateTime startime, DateTime endtime, Guid? id)
        {
            var absList = await _absrepository.GetAll().Where(x => x.StartDate <= endtime && x.EndDate >= startime && x.UserId.Equals(userId)&&x.Id!=id).ToListAsync();
            if (absList.Count() > 0)
                throw new ADTOSharpException(L("申请失败：你已有请假申请单包含该时间段"));
            var outList = await _outrepository.GetAll().Where(x => x.StartDate <= endtime && x.EndDate >= startime && x.UserID.Equals(userId) && x.Id != id).ToListAsync();
            if (outList.Count() > 0)
                throw new ADTOSharpException(L("申请失败：你已有外出申请单包含该时间段"));
            var onbList = await _outrepository.GetAll().Where(x => x.StartDate <= endtime && x.EndDate >= startime && x.UserID.Equals(userId) && x.Id != id).ToListAsync();
            if (onbList.Count() > 0)
                throw new ADTOSharpException(L("申请失败：你已有出差申请单包含该时间段"));
        }
        /// <summary>
        /// 判断时间-表单申请时判断申请时间是否在有效期内
        /// </summary>
        /// <param name="date">申请开始时间</param>
        /// <param name="type">表单类型（请假-Abs,出差-Onb,外出-Out,考勤异常-Att,ALL:全开（审核和申请））</param>
        /// <param name="isAudit">判断是审核还是申请</param>
        /// <returns></returns>
        public async Task<bool> IsDateValid(DateTime applyDate, string type, bool isAudit = false)
        {
            // applyDate申请时间，比较日期（applyDate 可以确保只比较日期部分，忽略时间）
            //如果申请月份与当前月份一样，则可以直接申请或者是审核
            //如果申请时间小于设置的月份则不能提交
            //申请时间applyDate 必须大于等于当前时间指定日和时分

            //截止时间
            var datetime = _appConfiguration["App:AttAbnormalApplyDatime"].ToDate();
            // 获取当前时间
            var currentTime = DateTime.Now;
            bool result = false;
            // 获取当前月份的第2天（时间部分为 00:00:00）
            var currentMonthDeadline = new DateTime(currentTime.Year, currentTime.Month, datetime.Day, datetime.Hour, datetime.Minute, 0);
            //如果申请月份小于当前月份，则判断当前时间满不满足截止时间
            if (applyDate.Year < currentTime.Year || applyDate.Month < currentTime.Month)
            {
                //如果申请的是上月的考勤，则判断当前时间是否大于截止申请时间
                result = currentTime > currentMonthDeadline;
            }
            else
            {
                return result;
            }

            //result ：true  则表示大于截止时间，需要去数据库里面查询特殊情况
            if (result)
            {
                string userName = "";
                var currentUser = await _userRepository.GetAsync(ADTOSharpSession.UserId.Value) ?? new User();
                if (!string.IsNullOrEmpty(currentUser.UserName))
                    userName = currentUser.UserName;
                //允许的日期必须要大于当前时间，才被允许申请
                var checkdateList = await _repository.GetAll().Where(d => d.IsAudit.Equals(isAudit) && d.EndDate >= currentTime
                && (d.UserName == userName || d.Type == "ALL"))
                    .WhereIf(string.IsNullOrWhiteSpace(type), q => q.Type.Contains(type)).ToListAsync();
                if (checkdateList.Count <= 0)
                    return true;
                else
                    return false;
            }
            return result;
        }
    }
}
