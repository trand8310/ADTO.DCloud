using ADTOSharp.Domain.Entities;
using System;
using System.ComponentModel;

namespace ADTO.DCloud.Surveys
{
    /// <summary>
    /// 考卷参与者
    /// </summary>
    public class SurveyParticipant : Entity<Guid>
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 考卷
        /// </summary>
        public Guid SurveyId { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        [Description("租户Id")]
        public Guid? TenantId { get; set; }

    }
}
