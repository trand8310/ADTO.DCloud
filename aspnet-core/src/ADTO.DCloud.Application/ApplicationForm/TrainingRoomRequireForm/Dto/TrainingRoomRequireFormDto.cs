using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm.TrainingRoomRequireForm.Dto
{
    [AutoMap(typeof(Adto_TrainingRoomRequireForm))]
    public class TrainingRoomRequireFormDto : FullAuditedEntityDto<Guid>
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
        /// 使用时间
        /// </summary>
        public DateTime SDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EDate { get; set; }

        /// <summary>
        /// 参与对象
        /// </summary>
        [StringLength(500)]
        public string Participants { get; set; }

        /// <summary>
        /// 使用设备
        /// </summary>
        [StringLength(128)]
        public string Equipment { get; set; }

        /// <summary>
        /// 会议室
        /// 字典
        /// </summary>
        [StringLength(128)]
        public string TrainingRoomName { get; set; }

        /// <summary>
        /// 会议室类型
        /// 字典
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 区域
        /// 字典
        /// </summary>
        public string Area { get; set; }
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
