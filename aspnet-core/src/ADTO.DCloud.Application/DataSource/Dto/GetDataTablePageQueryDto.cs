using ADTO.DCloud.Dto;
using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.DataSource.Dto
{
    /// <summary>
    /// 获取数据源数据分页查询
    /// </summary>
    public class GetDataTablePageQueryDto : PagedAndSortedInputDto
    {
        /// <summary>
        /// 编码 data/dbsource/{code}/page
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 查询参数
        /// </summary>
        public string whereSql { get; set; }

        /// <summary>
        /// 执行Sql （配置时，获取的sql） data/dbsource/view/page
        /// </summary>
        public string querySql { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public string param { get; set; }

        public int records { get; set; }

        //
        // 摘要:
        //     总页数
        public int total
        {
            get
            {
                if (records > 0)
                {
                    return (records % base.PageSize == 0) ? (records / base.PageSize) : (records / base.PageSize + 1);
                }

                return 0;
            }
        }
    }
}
