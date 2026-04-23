using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.CommodityStocks.Dto
{
    /// <summary>
    /// 电脑申请归还记录Dto
    /// </summary>
    public class StockReturnRecoredDto
    {
        /// <summary>
        /// 明细记录Id
        /// </summary>
        public Guid DetailId { get; set; }

        /// <summary>
        /// 申请类型
        /// </summary>
        public string ApplyType { get; set; }
        /// <summary>
        /// 申请数量
        /// </summary>
        public int ApplyCount { get; set; }

        /// <summary>
        /// 申请类型
        /// </summary>
        public string ApplyTypeName { get; set; }

        /// <summary>
        /// 产品Id
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 产品型号
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 产品分类
        /// </summary>
        public Guid Category { get; set; }

        /// <summary>
        /// 产品分类
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 产品编号
        /// </summary>
        public string SN { get; set; }

        /// <summary>
        /// 申请账号
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 申请用户
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 离职日期
        /// </summary>
        public DateTime? OutJobDate { get; set; }

        /// <summary>
        /// 用户状态
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 用户状态文本
        /// </summary>
        public string IsActiveText { get; set; }

        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 所属部门
        /// </summary>
        public string DepartName { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 公司号码
        /// </summary>
        public string CompanyTelephone { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// 归还时间
        /// </summary>
        public DateTime? BackTime { get; set; }

        /// <summary>
        /// 归还用户
        /// </summary>
        public string BackUser { get; set; }

        /// <summary>
        /// 归还用户
        /// </summary>
        public string BackUserId { get; set; }

        /// <summary>
        /// 归还备注
        /// </summary>
        public string BackRemark { get; set; }

        /// <summary>
        /// 归还状态
        /// </summary>
        public bool ReturnStatus { get; set; }

        /// <summary>
        /// 归还状态文本
        /// </summary>
        public string ReturnStatusText { get; set; }

        /// <summary>
        /// 申请Adto_ProductApply Id
        /// </summary>
        public Guid ProductApplyId { get; set; }

    }
}
