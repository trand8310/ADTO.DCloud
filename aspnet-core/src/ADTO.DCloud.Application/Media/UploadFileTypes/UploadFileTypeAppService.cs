using System;
using System.Linq;
using ADTOSharp.UI;
using System.Threading.Tasks;
using System.Collections.Generic;
using ADTOSharp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Application.Services.Dto;
using ADTO.DCloud.Media.UploadFileTypes.Dto;
using ADTOSharp.Collections.Extensions;

namespace ADTO.DCloud.Media.UploadFileTypes
{
    /// <summary>
    /// 文件、图片类型
    /// </summary>
    public class UploadFileTypeAppService : DCloudAppServiceBase, IUploadFileTypeAppService
    {
        private readonly IRepository<UploadFileType, Guid> _uploadFileTypeRepository;
        public UploadFileTypeAppService(IRepository<UploadFileType, Guid> uploadFileTypeRepository)
        {
            _uploadFileTypeRepository = uploadFileTypeRepository;
        }

        /// <summary>
        /// 添加文件、图片类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateFileTypeAsync(CreateUploadFileTypeDto input)
        {
            var dataItem = ObjectMapper.Map<UploadFileType>(input);
            await _uploadFileTypeRepository.InsertAsync(dataItem);
        }

        /// <summary>
        /// 修改文件、图片类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateFileTypeAsync(CreateUploadFileTypeDto input)
        {
            var info = this._uploadFileTypeRepository.Get(input.Id.Value);
            ObjectMapper.Map(input, info);

            //转换一下，否则其它字段也会置空
            await _uploadFileTypeRepository.UpdateAsync(info);
        }

        /// <summary>
        /// 删除文件、图片类型
        /// </summary>
        /// <param name="Id"></ param>
        /// <returns></returns>
        public async Task DeleteFileTypeAsync(EntityDto<Guid> input)
        {
            var info = await this._uploadFileTypeRepository.GetAll().Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            if (info == null)
            {
                throw new UserFriendlyException("操作失败，记录不存在！");
            }
            var paretCount = await _uploadFileTypeRepository.CountAsync(p => p.ParentId == input.Id);
            if (paretCount > 0)
            {
                throw new Exception("操作失败：存在子节点，不允许删除");
            }
            await _uploadFileTypeRepository.DeleteAsync(input.Id);
        }

        /// <summary>
        /// 根据ID获取文件、图片类型详细信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<UploadFileTypeDto> GetFileTypeById(EntityDto<Guid> input)
        {
            var info = await _uploadFileTypeRepository.GetAsync(input.Id);
            var infoDto = ObjectMapper.Map<UploadFileTypeDto>(info);
            return infoDto;
        }

        /// <summary>
        /// 获取所有的文件、图片类型列表信息
        /// </summary>
        /// <param name="IsAll"></param>
        /// <returns></returns>
        public async Task<List<UploadFileTypeDto>> GetFileTypeList(bool IsAll)
        {
            var query = _uploadFileTypeRepository.GetAll();
            if (!IsAll)
            {
                query = query.Where(p => p.IsActive);
            }
            var list = await query.ToListAsync();
            return ObjectMapper.Map<List<UploadFileTypeDto>>(list);
        }

        /// <summary>
        /// 文件、图片类型树结构菜单
        /// </summary>
        /// <returns></returns>
        public async Task<List<UploadFileTypeDto>> GetFileTypeTreeList(PagedFileTypeResultRequestDto input)
        {
            var query = _uploadFileTypeRepository.GetAll()
                .WhereIf(!string.IsNullOrWhiteSpace(input.ProjectClassName), p => p.ProjectClassName == input.ProjectClassName)
                .WhereIf(input.ProjectId != null && input.ProjectId != Guid.Empty, p => p.ProjectId == input.ProjectId)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Title), p => p.Title.Contains(input.Title));

            if (!input.IsAll)
            {
                query = query.Where(p => p.IsActive);
            }
            var list = await query.ToListAsync();

            var modules = list.Select(item =>
            {
                var moduleDto = ObjectMapper.Map<UploadFileTypeDto>(item);
                if (item.ParentId != null && item.ParentId != Guid.Empty)
                {
                    var Parent = this._uploadFileTypeRepository.GetAll().Where(p => p.Id == item.ParentId).FirstOrDefault();
                    if (Parent != null)
                    {
                        moduleDto.Parent = ObjectMapper.Map<UploadFileTypeDto>(Parent);
                    }
                }
                return moduleDto;
            }).ToList();
            var data = this.InternalGetTreeList(modules, null);
            return data;
        }

        private List<UploadFileTypeDto> InternalGetTreeList(List<UploadFileTypeDto> list, Guid? parentId = null)
        {
            var query = list.AsQueryable()
                .WhereIf(parentId != null && parentId != Guid.Empty, w => w.Parent != null && w.ParentId == parentId)
                .WhereIf(parentId == null || parentId == Guid.Empty, w => w.Parent == null)
                .OrderBy(o => o.DisplayOrder).ThenBy(o => o.CreationTime).ToList();
            return query.Select(item =>
            {
                var m = ObjectMapper.Map<UploadFileTypeDto>(item);
                m.Children = InternalGetTreeList(list, item.Id);
                return m;
            }).ToList();
        }
    }
}

