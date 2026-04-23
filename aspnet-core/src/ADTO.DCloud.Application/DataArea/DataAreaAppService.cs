using System.Linq;
using ADTO.DCloud.DataArea.Dto;
using System.Threading.Tasks;
using ADTOSharp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.DataArea
{
    public class DataAreaAppService : DCloudAppServiceBase, IDataAreaAppService
    {
        private readonly IRepository<DataArea, string> _dataAreaRepository;
        public DataAreaAppService(IRepository<DataArea, string> dataAreaRepository)
        {
            _dataAreaRepository = dataAreaRepository;
        }

        /// <summary>
        /// 获取区域列表 data/areas/{pid}
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ListResultDto<DataAreaDto>> GetDataAreaListAsync(DataAreaListQueryDto input)
        {
            var query = this._dataAreaRepository.GetAll();
            if (string.IsNullOrWhiteSpace(input.ParentId))
            {
                input.ParentId = "0";
            }
            query = query.Where(p => p.ParentId == input.ParentId);
            var list = await query.ToListAsync();
            return new ListResultDto<DataAreaDto>(ObjectMapper.Map<List<DataAreaDto>>(list));
        }
    }
}

