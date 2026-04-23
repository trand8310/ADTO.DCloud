using System;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.Domain.Entities.Auditing;

namespace ADTO.DCloud.Tasks.Dto
{
    /// <summary>
    /// 系统任务
    /// </summary>
    [AutoMap(typeof(TaskScheduler))]
    public class TaskSchedulerDto : EntityDto<Guid>, IHasCreationTime
    {
        //标题
        public string Title { get; set; }

        /// <summary>
        /// URL地址或者服务名
        /// </summary>
        public string ExecuteName { get; set; }

        /// <summary>
        /// 任务状态 true=开启；false=关闭
        /// </summary>
        public bool State { get; set; }

        /// <summary>
        /// 周期类型（字典表）
        /// </summary>
        public string CycleType { get; set; }

        /// <summary>
        /// 周期Json 值
        /// 拼接的json 值，有年月日、有小时、分钟，类型种类太多，字段不统一
        /// </summary>
        public string CycleJsonValue { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public DateTime CreationTime { get ; set ; }

        /// <summary>
        /// 最后执行时间
        /// </summary>
        public DateTime? LastExecutionTime { get; set; }

        /// <summary>
        /// 下次执行时间
        /// </summary>
        public DateTime? NextExecutionTime { get; set; }
    }
}
