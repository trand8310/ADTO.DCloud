namespace ADTO.DCloud.Media.FileManage.Aliyun.Dto
{
    /// <summary>
    /// 阿里云密钥信息
    /// </summary>
    public class AliyunAccessKeyDto
    {
        /// <summary>
        /// accessKeyId
        /// </summary>
        public string accessKeyId { get; set; }

        /// <summary>
        /// accessKeySecret
        /// </summary>
        public string accessKeySecret { get; set; }

        /// <summary>
        /// bucket名称
        /// </summary>
        public string bucketName { get; set; }

        /// <summary>
        /// endpoint
        /// </summary>
        public string endpoint { get; set; }

        /// <summary>
        /// Bucket所在地域
        /// </summary>
        public string region { get; set; }
    }
}
