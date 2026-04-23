using ADTO.DCloud.Authorization.Users;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Processs.Dto
{
    /// <summary>
    /// 流程用户
    /// </summary>
    public class WFUserInfo:EntityDto<Guid>
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 公司
        /// </summary>
        public Guid CompanyId { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public Guid DepartmentId { get; set; }

        /// <summary>
        /// 岗位
        /// </summary>
        public Guid PostId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName { get; set; }
        /// <summary>
        /// 审批顺序
        /// </summary>
        public int? Sort { get; set; }
        /// <summary>
        /// 是否需要审批
        /// </summary>
        public bool IsAwait { get; set; }
        /// <summary>
        /// 是否没有审批人
        /// </summary>
        public bool NotHasUser { get; set; }
        /// <summary>
        /// 上一次审批人是否同意（这边的定义为除了驳回的）
        /// </summary>
        public bool IsAgree { get; set; }

        /// <summary>
        /// 是否是超级管理员
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// 任务状态 1.激活 2.待激活 3.完成 4.关闭 5.加签状态 6.转办给其他人 7.作废 8.子流程运行中
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 子流程id
        /// </summary>
        public Guid ChildProcessId { get; set; }

        /// <summary>
        /// 是否是加签
        /// </summary>
        public bool IsSign { get; set; }
    }
}
