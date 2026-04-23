using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADTOSharp.AutoMapper;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.Tasks.Dto
{
    [AutoMap(typeof(TaskExecutionHistory))]
    public class TaskExecutionHistoryDto : EntityDto<Guid>
    {
        /// <summary>
        /// 任务主键Id
        /// </summary>
        public Guid TaskSchedulerId { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 是否执行成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 执行结果
        /// </summary>
        [MaxLength(2000)]
        public string Result { get; set; }

        /// <summary>
        /// 错误详情
        /// </summary>
        [MaxLength(2000)]
        public string ErrorMessage { get; set; }
    }
}
