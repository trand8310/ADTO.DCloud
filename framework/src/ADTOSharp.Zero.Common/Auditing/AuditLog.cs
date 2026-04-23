using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Extensions;
using ADTOSharp.Runtime.Validation;
using ADTOSharp.UI;

namespace ADTOSharp.Auditing
{
    /// <summary>
    /// 审计日志
    /// </summary>
    [Table("ADTOSharpAuditLogs"), Description("审计日志")]
    public class AuditLog : Entity<long>, IMayHaveTenant
    {
        /// <summary>
        /// Maximum length of <see cref="ServiceName"/> property.
        /// </summary>
        public static int MaxServiceNameLength = 256;

        /// <summary>
        /// Maximum length of <see cref="MethodName"/> property.
        /// </summary>
        public static int MaxMethodNameLength = 256;

        /// <summary>
        /// Maximum length of <see cref="Parameters"/> property.
        /// </summary>
        public static int MaxParametersLength = 2048;

        /// <summary>
        /// Maximum length of <see cref="ReturnValue"/> property.
        /// </summary>
        public static int MaxReturnValueLength = 1024;

        /// <summary>
        /// Maximum length of <see cref="ClientIpAddress"/> property.
        /// </summary>
        public static int MaxClientIpAddressLength = 64;

        /// <summary>
        /// Maximum length of <see cref="ClientName"/> property.
        /// </summary>
        public static int MaxClientNameLength = 128;

        /// <summary>
        /// Maximum length of <see cref="BrowserInfo"/> property.
        /// </summary>
        public static int MaxBrowserInfoLength = 512;

        /// <summary>
        /// Maximum length of <see cref="ExceptionMessage"/> property.
        /// </summary>
        public static int MaxExceptionMessageLength = 1024;

        /// <summary>
        /// Maximum length of <see cref="Exception"/> property.
        /// </summary>
        public static int MaxExceptionLength = 2000;

        /// <summary>
        /// Maximum length of <see cref="CustomData"/> property.
        /// </summary>
        public static int MaxCustomDataLength = 2000;

        /// <summary>
        /// 租户Id.
        /// </summary>
        public virtual Guid? TenantId { get; set; }

        /// <summary>
        /// 用户Id.
        /// </summary>
        public virtual Guid? UserId { get; set; }

        /// <summary>
        /// 服务名称(接口/类名).
        /// </summary>
        public virtual string ServiceName { get; set; }

        /// <summary>
        /// 方法名称.
        /// </summary>
        public virtual string MethodName { get; set; }

        /// <summary>
        /// 参数列表
        /// </summary>
        public virtual string Parameters { get; set; }

        /// <summary>
        /// 返回值
        /// </summary>
        public virtual string ReturnValue { get; set; }

        /// <summary>
        /// 方法执行的开始时间
        /// </summary>
        public virtual DateTime ExecutionTime { get; set; }

        /// <summary>
        /// 方法执行的结束时间（以毫秒计）
        /// </summary>
        public virtual int ExecutionDuration { get; set; }

        /// <summary>
        /// 客户端的IP地址
        /// </summary>
        public virtual string ClientIpAddress { get; set; }

        /// <summary>
        /// 客户端的名称（一般为计算机名称）
        /// </summary>
        public virtual string ClientName { get; set; }

        /// <summary>
        /// 如果在web请求中调用此方法，则获取浏览器信息。
        /// </summary>
        public virtual string BrowserInfo { get; set; }

        /// <summary>
        /// 异常消息  <see cref="Exception"/>.
        /// </summary>
        public virtual string ExceptionMessage { get; set; }

        /// <summary>
        /// 异常对象，如果在方法执行期间发生异常。
        /// </summary>
        public virtual string Exception { get; set; }

        /// <summary>
        /// 模拟用户Id <see cref="AuditInfo.ImpersonatorUserId"/>.
        /// </summary>
        public virtual Guid? ImpersonatorUserId { get; set; }

        /// <summary>
        /// 模拟租户Id <see cref="AuditInfo.ImpersonatorTenantId"/>.
        /// </summary>
        public virtual Guid? ImpersonatorTenantId { get; set; }

        /// <summary>
        /// 用户数据 <see cref="AuditInfo.CustomData"/>.
        /// </summary>
        public virtual string CustomData { get; set; }

        /// <summary>
        /// 从给定信息创建一个新的 AuditInfo <paramref name="auditInfo"/>.
        /// </summary>
        /// <param name="auditInfo">Source <see cref="AuditInfo"/> object</param>
        /// <returns>The <see cref="AuditLog"/> object that is created using <paramref name="auditInfo"/></returns>
        public static AuditLog CreateFromAuditInfo(AuditInfo auditInfo)
        {
            var exceptionMessage = GetADTOSharpClearException(auditInfo.Exception);
            return new AuditLog
            {
                TenantId = auditInfo.TenantId,
                UserId = auditInfo.UserId,
                ServiceName = auditInfo.ServiceName.TruncateWithPostfix(MaxServiceNameLength),
                MethodName = auditInfo.MethodName.TruncateWithPostfix(MaxMethodNameLength),
                Parameters = auditInfo.Parameters.TruncateWithPostfix(MaxParametersLength),
                ReturnValue = auditInfo.ReturnValue.TruncateWithPostfix(MaxReturnValueLength),
                ExecutionTime = auditInfo.ExecutionTime,
                ExecutionDuration = auditInfo.ExecutionDuration,
                ClientIpAddress = auditInfo.ClientIpAddress.TruncateWithPostfix(MaxClientIpAddressLength),
                ClientName = auditInfo.ClientName.TruncateWithPostfix(MaxClientNameLength),
                BrowserInfo = auditInfo.BrowserInfo.TruncateWithPostfix(MaxBrowserInfoLength),
                Exception = exceptionMessage.TruncateWithPostfix(MaxExceptionLength),
                ExceptionMessage = auditInfo.Exception?.Message.TruncateWithPostfix(MaxExceptionMessageLength),
                ImpersonatorUserId = auditInfo.ImpersonatorUserId,
                ImpersonatorTenantId = auditInfo.ImpersonatorTenantId,
                CustomData = auditInfo.CustomData.TruncateWithPostfix(MaxCustomDataLength)
            };
        }

        public override string ToString()
        {
            return string.Format(
                "AUDIT LOG: {0}.{1} is executed by user {2} in {3} ms from {4} IP address.",
                ServiceName, MethodName, UserId, ExecutionDuration, ClientIpAddress
            );
        }

        /// <summary>
        /// 格式化显示异常信息。
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string GetADTOSharpClearException(Exception exception)
        {
            var clearMessage = "";
            switch (exception)
            {
                case null:
                    return null;

                case ADTOSharpValidationException adtoValidationException:
                    clearMessage = "There are " + adtoValidationException.ValidationErrors.Count + " validation errors:";
                    foreach (var validationResult in adtoValidationException.ValidationErrors)
                    {
                        var memberNames = "";
                        if (validationResult.MemberNames != null && validationResult.MemberNames.Any())
                        {
                            memberNames = " (" + string.Join(", ", validationResult.MemberNames) + ")";
                        }

                        clearMessage += "\r\n" + validationResult.ErrorMessage + memberNames;
                    }

                    break;

                case UserFriendlyException userFriendlyException:
                    clearMessage =
                        $"UserFriendlyException.Code:{userFriendlyException.Code}\r\nUserFriendlyException.Details:{userFriendlyException.Details}";
                    break;
            }

            return exception + (clearMessage.IsNullOrWhiteSpace() ? "" : "\r\n\r\n" + clearMessage);
        }
    }
}
