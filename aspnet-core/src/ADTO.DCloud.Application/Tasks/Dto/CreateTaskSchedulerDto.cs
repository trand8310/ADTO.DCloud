using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Tasks.Dto
{
    /// <summary>
    /// 添加系统任务配置
    /// </summary>
    [AutoMapTo(typeof(TaskScheduler))]
    public class CreateTaskSchedulerDto : EntityDto<Guid?>
    {
        //标题
        public string Title { get; set; }

        /// <summary>
        /// URL地址
        /// </summary>
        public string Url { get; set; }

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

    }
}
