using System;
using ADTOSharp.Authorization;
using ADTOSharp.Runtime.Validation;
using ADTO.DCloud.Dto;

namespace ADTO.DCloud.Authorization.Users.Dto
{
    public class GetLoginAttemptsInput: PagedAndSortedInputDto, IGetLoginAttemptsInput, IShouldNormalize
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string Filter { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 截束时间
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 登录状态
        /// </summary>
        public ADTOSharpLoginResultType? Result { get; set; }
        
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime DESC";
            }

            Filter = Filter?.Trim();
        }
    }
}
