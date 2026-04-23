using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm.UseCar.Dto
{
    [AutoMap(typeof(Adto_UseCar))]
    public class UseCarDto:FullAuditedEntityDto<Guid>
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 公司Id
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        public Guid DepartmentId { get; set; }


        /// <summary>
        /// 标题
        /// </summary>
        [StringLength(225)]
        public string Title { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime SDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime EDate { get; set; }

        [StringLength(500)]
        public string Reason { get; set; }

        /// <summary>
        /// 起始公里
        /// </summary>
        public decimal SMileage { get; set; }

        /// <summary>
        /// 结束公里
        /// </summary>
        public decimal EMileage { get; set; }

        /// <summary>
        /// 申请理由
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }

        /// <summary>
        /// 乘车人数
        /// </summary>
        public int PassengerNum { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string CarId { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        [StringLength(128)]
        public string CarNum { get; set; }

        /// <summary>
        /// 日期
        /// 老系统用于申请日期
        /// </summary>
        public DateTime? ApplyDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? States { get; set; }

        /// <summary>
        /// 路线
        /// </summary>
        [StringLength(500)]
        public string Route { get; set; }

        /// <summary>
        /// 用车类型
        /// 数据字典
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 油费
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// 司机
        /// </summary>
        [StringLength(128)]
        public string Driver { get; set; }

        /// <summary>
        /// 返回时间
        /// </summary>
        public DateTime EstimateReturnDate { get; set; }

        /// <summary>
        /// 起始公里图
        /// </summary>
        [StringLength(128)]
        public string SImage { get; set; }

        /// <summary>
        /// 结束公里图
        /// </summary>
        [StringLength(128)]
        public string EImage { get; set; }

        #region 扩展字段
        /// <summary>
        /// 用户工号
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        public string DepartmentName { get; set; }
        /// <summary>
        /// 流程进程是否结束1是0不是
        /// </summary>
        public int IsFinished { get; set; }
        /// <summary>
        /// 流程编码
        /// </summary>
        public string SchemeCode { get; set; }
        #endregion
    }
}
