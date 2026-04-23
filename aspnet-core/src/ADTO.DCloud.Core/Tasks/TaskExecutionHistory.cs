using System;
using ADTOSharp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.Tasks
{
    /// <summary>
    /// 任务执行历史日志
    /// </summary>

    public class TaskExecutionHistory : Entity<Guid>
    {
        /// <summary>
        /// 任务主键Id
        /// </summary>
        [ForeignKey("TaskSchedulerId")]
        [Description("任务主键Id")]
        public Guid TaskSchedulerId { get; set; }
        public virtual TaskScheduler TaskScheduler { get; set; }

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
