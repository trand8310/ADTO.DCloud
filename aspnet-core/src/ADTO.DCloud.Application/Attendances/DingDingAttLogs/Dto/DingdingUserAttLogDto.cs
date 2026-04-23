using ADTO.DCloud.Attendances;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Attendances.DingDingAttLogs.Dto
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMap(typeof(DingdingUserAttLog))]
    public class DingdingUserAttLogDto : EntityDto<Guid>
    {
        /// <summary>
        /// 头像
        /// </summary>
        [StringLength(225)]
        public string Avatar { get; set; }
        /// <summary>
        /// 签到详细地址
        /// </summary>
        [StringLength(225)]
        public string DetailPlace { get; set; }
        /// <summary>
        /// 图片集合
        /// </summary>
        [StringLength(225)]
        public string ImageLists { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        [StringLength(200)]
        public string Latitude { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        [StringLength(200)]
        public string Longitude { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        [StringLength(225)]
        public string Name { get; set; }
        /// <summary>
        /// 签到地址
        /// </summary>
        [StringLength(225)]
        public string Place { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }
        /// <summary>
        /// 签到时间
        /// </summary>
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        [StringLength(128)]
        public string UserName { get; set; }
        /// <summary>
        /// 是否算正常考勤
        /// </summary>
        public int? IsNormal { get; set; }
        /// <summary>
        /// 是否默认考勤地址
        /// </summary>
        public int? IsDefaultAddr { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
        /// <summary>
        /// 打卡类型-默认0是钉钉签到1是考勤打卡
        /// </summary>
        public string Type { get; set; }
    }
}
