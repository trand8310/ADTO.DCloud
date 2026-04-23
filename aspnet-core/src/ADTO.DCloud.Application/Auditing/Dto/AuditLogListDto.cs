using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Auditing;
using ADTOSharp.AutoMapper;
using System;
 

namespace ADTO.DCloud.Auditing.Dto
{
    //### This class is mapped in CustomDtoMapper ###
    [AutoMapFrom(typeof(AuditLog))]
    public class AuditLogListDto : EntityDto<long>
    {
        /// <summary>
        /// UserId
        /// </summary>
        public Guid? UserId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 模拟租户
        /// </summary>
        public Guid? ImpersonatorTenantId { get; set; }
        /// <summary>
        /// 模拟用户
        /// </summary>
        public Guid? ImpersonatorUserId { get; set; }
        /// <summary>
        /// 服务
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// 方法
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public string Parameters { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime ExecutionTime { get; set; }
        /// <summary>
        /// 执行时长
        /// </summary>
        public int ExecutionDuration { get; set; }
        /// <summary>
        /// 客户端IP
        /// </summary>
        public string ClientIpAddress { get; set; }
        /// <summary>
        /// 客户端名称
        /// </summary>
        public string ClientName { get; set; }
        /// <summary>
        /// 浏览器信息
        /// </summary>
        public string BrowserInfo { get; set; }
        /// <summary>
        /// 异常信息
        /// </summary>
        public string Exception { get; set; }
        /// <summary>
        /// 自定义数据
        /// </summary>
        public string CustomData { get; set; }
    }
}