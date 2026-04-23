using ADTO.DCloud.WorkFlow.StampManage;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Stamps.Dto
{
    [AutoMapTo(typeof(WorkFlowStamp))]
    public class UpdateStampInput:EntityDto<Guid>
    {
        #region 实体成员
        /// <summary>
        /// 印章名称
        /// </summary>
        [StringLength(100)]
        public string StampName { get; set; }
        /// <summary>
        /// 印章分类
        /// </summary>
        [StringLength(100)]
        public string StampType { get; set; }
        /// <summary>
        /// 是否启用密码 1 是 0 不是
        /// </summary>
        public int? IsNotPassword { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [StringLength(50)]
        public string Password { get; set; }
        /// <summary>
        /// 图片文件
        /// </summary>
        [StringLength(100)]
        public string ImgFile { get; set; }
        /// <summary>
        /// 关联用户（默认是创建用户）
        /// </summary>
        [StringLength(500)]
        public string UserIds { get; set; }
        /// <summary>
        /// 印章状态
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(200)]
        public string Remark { get; set; }
        #endregion
    }
}
