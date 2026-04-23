using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.Modules.Dto;
using ADTO.DCloud.WorkFlow.Schemes;
using ADTO.DCloud.WorkFlow.Schemes.Dto;
using ADTO.DCloud.WorkFlow.StampManage;
using ADTO.DCloud.WorkFlow.Stamps.Dto;
using ADTOSharp;
using ADTOSharp.Application.Services;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.ObjectMapping;
using AutoMapper.Internal.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Stamps
{
    /// <summary>
    /// 签章管理
    /// </summary>
    [ADTOSharpAuthorize]
    public class StampAppService : DCloudAppServiceBase, IStampAppService
    {
        private readonly IRepository<WorkFlowStamp, Guid> _stampRepository;
        public StampAppService(IRepository<WorkFlowStamp, Guid> stampRepository)
        {
            _stampRepository = stampRepository;
        }

        #region 获取数据
        /// <summary>
        /// 获取分页列表签章数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<StampDto>> GetAllAsync(GetStampInput input)
        {
            var query = _stampRepository.GetAll().WhereIf(!string.IsNullOrWhiteSpace(input.KeyWord), q => q.StampName.Contains(input.KeyWord));
            var totalCount = query.Count();
            var list = await query.PageBy(input).ToListAsync();
            var listDtos = ObjectMapper.Map<List<StampDto>>(list);
            return new PagedResultDto<StampDto>(totalCount, listDtos);
        }

        /// <summary>
        /// 获取流程设计(依流程Id)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<StampDto> GetAsync(EntityDto<Guid> input)
        {
            var scheme = await _stampRepository.GetAsync(input.Id);
            return ObjectMapper.Map<StampDto>(scheme);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 新增签章
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<StampDto> CreateStampAsync(CreateStampInput input)
        {
            var stamp = ObjectMapper.Map<WorkFlowStamp>(input);
            await _stampRepository.InsertAsync(stamp);
            CurrentUnitOfWork.SaveChanges();
            return ObjectMapper.Map<StampDto>(stamp);
        }
        /// <summary>
        /// 修改签章
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<StampDto> UpdateStampleAsync(UpdateStampInput input)
        {
            var stamp = await _stampRepository.GetAsync(input.Id);
            ObjectMapper.Map(input, stamp);
            await _stampRepository.UpdateAsync(stamp);
            return await GetAsync(new EntityDto<Guid>() { Id = stamp.Id });
        }
        /// <summary>
        /// 删除签章
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[HttpDelete("workflow/scheme/{id}")]
        public async Task DeleteStampAsync(EntityDto<Guid> input)
        {
            await _stampRepository.DeleteAsync(input.Id);
        }
        #endregion


        /// <summary>
        /// 签章图片归档并验证密码
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public async Task<string> ToWfImg(string keyValue, string password)
        {
            if (keyValue == null || keyValue.IsEmpty())
            {
                return "";
            }
            WorkFlowStamp entity = await _stampRepository.GetAsync(Guid.Parse(keyValue));
            if (entity != null && (entity.IsNotPassword == 0 || entity.Password.Equals(password)))
            {
                var imgId = entity.ImgFile;
                var newImgId = $"adto_{imgId}";

                //string path = await _annexesFileIBLL.DownFile(imgId);
                //if (!string.IsNullOrEmpty(path))
                //{
                //    await _annexesFileIBLL.SaveAnnexes(newImgId, newImgId, path, "", "", "");
                //}
                //else
                //{
                //    return string.Empty;
                //}
                //return newImgId;
                return "";
            }
            else
            {
                return "";
            }
        }
    }
}

