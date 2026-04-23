using System;
using System.Threading.Tasks;

namespace ADTO.DCloud.DataBase.Location
{
    /// <summary>
    /// 大区、国家、省市区
    /// </summary>
    public interface ILocationAppService
    {
        /// <summary>
        /// 实现获取区域最后一级中文名称(因为层级不确定)
        /// </summary>
        /// <param name="areaId"></param>
        /// <param name="countryId"></param>
        /// <param name="provinceId"></param>
        /// <param name="cityId"></param>
        /// <param name="countyId"></param>
        /// <returns></returns>
        Task<string> GetLastLevelNameAsync(Guid areaId, Guid countryId, Guid? provinceId, Guid? cityId, Guid? countyId);
    }
}
