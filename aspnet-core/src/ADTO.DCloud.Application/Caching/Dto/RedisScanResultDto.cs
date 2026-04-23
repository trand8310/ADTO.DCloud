using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Caching.Dto
{
    public class RedisScanResultDto
    {
        public long Cursor { get; set; }
        public List<RedisKeyItemDto> Items { get; set; } = new();
    }

    public class RedisKeyItemDto
    {
        public string Key { get; set; }
        public double? TtlSeconds { get; set; }
        public string Value { get; set; } // 可选
    }

}
