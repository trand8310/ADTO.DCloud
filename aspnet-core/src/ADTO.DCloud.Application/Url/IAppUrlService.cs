using System;

namespace ADTO.DCloud.Url;

public interface IAppUrlService
{
    /// <summary>
    /// 邮件地址激活url
    /// </summary>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    string CreateEmailActivationUrlFormat(Guid? tenantId);
    /// <summary>
    /// 创建邮件变更请求url
    /// </summary>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    string CreateEmailChangeRequestUrlFormat(Guid? tenantId);
    /// <summary>
    /// 创建密码重置url
    /// </summary>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    string CreatePasswordResetUrlFormat(Guid? tenantId);
    /// <summary>
    /// 租户名,邮件地址激活url
    /// </summary>
    /// <param name="tenancyName"></param>
    /// <returns></returns>
    string CreateEmailActivationUrlFormat(string tenancyName);
    /// <summary>
    /// 租户名 创建密码重置url
    /// </summary>
    /// <param name="tenancyName"></param>
    /// <returns></returns>
    string CreatePasswordResetUrlFormat(string tenancyName);
}
