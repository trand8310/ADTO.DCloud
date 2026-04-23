using System;
namespace ADTO.DCloud.CommodityStocks.PresidentofficeStock.Dto
{
    /// <summary>
    /// 总裁办库存物品申请名称
    /// </summary>
    public class OfficeSupplyApplicationDetailDto
    {
        /// <summary>
        /// 物品明细Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 申请标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 申请明细Id(扣费根据这个Id)
        /// </summary>
        public Guid ProductAppyDetailId { get; set; }
        /// <summary>
        /// 物品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        ///物品Id
        /// </summary>
        public Guid ProductId { get; set; }
        /// <summary>
        /// 物品类别
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// 类别名称
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 物品型号
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// ApplyType 、物品扣费状态
        /// </summary>
        /// <returns></returns>
        public int? ApplyType { get; set; }
        /// <summary>
        /// 申请状态
        /// </summary>
        public string ApplyTypeText { get; set; }

        /// <summary>
        /// 归还人
        /// </summary>
        public Guid? LastModifierUserId { get; set; }

        /// <summary>
        /// 申请数量
        /// </summary>
        public int? ApplyCount { get; set; }
        /// <summary>
        /// 物品价格
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 物品总价
        /// </summary>
        public decimal TotalPrice { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime CreationTime { get; set; }
        /// <summary>
        /// 归还人
        /// </summary>
        public string BackUser { get; set; }
        /// <summary>
        /// 归还时间
        /// </summary>
        public DateTime? BackTime { get; set; }
        /// <summary>
        /// 申请备注
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 申请明细备注
        /// </summary>
        public string Remarks { get; set; }
    }
}
