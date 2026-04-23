using ADTO.DCloud.Dto;
using ADTO.DCloud.Infrastructure;
using ADTOSharp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Caching.Dto
{
    public class GetCachesDto
    {
        public GetCachesDto()
        {
            Keyword = "*";
            ShowValue = false;
            PageSize = 20;
        }
        /// <summary>
        /// 游标值,方便服务端扫描KEY值,下一页的游标值须由客户端返回的上一次查询获取,该值不能自定义除非0外的其它值
        /// </summary>
        public long Cursor { get; set; }
        /// <summary>
        /// 每页显示的KEY值的数量
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string Keyword { get; set; }
        /// <summary>
        ///  是否显示值
        /// </summary>
        public bool ShowValue { get; set; }
    }
}
