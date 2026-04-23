using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Media.FileManage.Aliyun.Dto
{
    /// <summary>
    /// 临时身份凭证
    /// </summary>
    public class AssumeRoleResutlDto
    {
        /// <summary>
        /// 请求 ID。
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// 角色扮演时的临时身份
        /// </summary>
        public AssumedRoleUser assumedRoleUser { get; set; }

        /// <summary>
        /// 访问凭证
        /// </summary>
        public Credentials credentials { get; set; }
    }

    /// <summary>
    /// 角色扮演时的临时身份
    /// </summary>
    public class AssumedRoleUser
    {
        /// <summary>
        /// 临时身份的 ID。
        /// </summary>
        public string AssumedRoleId { get; set; }

        /// <summary>
        /// 临时身份的 ARN
        /// </summary>
        public string Arn { get; set; }
    }

    /// <summary>
    /// 访问凭证
    /// </summary>
    public class Credentials
    {
        /// <summary>
        /// 安全令牌
        /// </summary>
        public string SecurityToken { get; set; }

        /// <summary>
        /// Token 到期失效时间（UTC 时间）
        /// </summary>
        public string Expiration { get; set; }

        /// <summary>
        /// 访问密钥
        /// </summary>
        public string AccessKeySecret { get; set; }

        /// <summary>
        /// 访问密钥 ID
        /// </summary>
        public string AccessKeyId { get; set; }


    }
}
