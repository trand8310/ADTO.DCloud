using System;

namespace ADTO.DCloud.Customers.Dto
{
    /// <summary>
    /// 新增、修改客户时，查重参数
    /// </summary>
    public class CustomerExistInfoDto
    {
        /// <summary>
        /// 客户Id(客户编辑必传)
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 客户邮箱
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }
}
