using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Tasks;

/// <summary>
/// 任务的状态 <see cref="Task"/>.
/// </summary>
public enum TaskState : byte
{
    /// <summary>
    /// 默认状态
    /// </summary>
    All = 0,

    /// <summary>
    /// 任务已完成
    /// </summary>
    Active = 1,

    /// <summary>
    /// 任务已完成
    /// </summary>
    Completed = 2
}
