
using System;

namespace ADTO.DCloud.Training.Dto
{
    /// <summary>
    /// 培训记录汇总
    /// </summary>
    public class TrainingArchivesCountDto
    {
        /// <summary>
        /// 培训标题
        /// </summary>
        public string TrainingTitle { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 用户工号
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 公司Id
        /// </summary>
        public Guid? CompanyId { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        public Guid DeptId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DeptName { get; set; }

        /// <summary>
        /// 职级
        /// </summary>
        public string? PostLevelId { get; set; }

        /// <summary>
        /// 职级名称
        /// </summary>
        public string PostLevelName { get; set; }

        /// <summary>
        /// 标准学时（默认3）
        /// </summary>
        public int StandardLearningTime { get; set; }

        /// <summary>
        /// 学时汇总
        /// </summary>
        public decimal LearningTimeCount { get; set; }

        /// <summary>
        /// 学分汇总
        /// </summary>
        public decimal ScoreCount { get; set; }

        /// <summary>
        /// 达标率 
        /// </summary>
        public decimal ComplianceRate { get; set; }

        /// <summary>
        /// 培训月份
        /// </summary>
        public int TrainingMonth { get; set; }
    }
}
