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
    [AutoMapTo(typeof(Adto_TrainingRoomRequireForm))]
    public class CreateTrainingRoomRequireFormDto : FullAuditedEntityDto<Guid>, ITrainingRoomDto
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
    }
}
