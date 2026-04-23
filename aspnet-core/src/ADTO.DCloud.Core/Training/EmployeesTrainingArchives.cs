using ADTO.DCloud.Authorization.Users;
using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADTO.DCloud.Training
{
    /// <summary>
    /// 员工培训记录
    /// </summary>
    [Table("EmployeesTrainingArchives")]
    public class EmployeesTrainingArchives: FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid UserId { get; set; }
       
        /// <summary>
        /// 流程信息表Id
        /// </summary>
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        /// <summary>
        /// 公司Id
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        public Guid DeptId { get; set; }

        /// <summary>
        /// 职级
        /// </summary>
        public string PostLevelId { get; set; }

        /// <summary>
        /// 培训日期
        /// </summary>
        public DateTime? TrainingDate { get; set; }

        /// <summary>
        /// 培训标题
        /// </summary>
        public string TrainingTitle { get; set; }

        /// <summary>
        /// 培训课时
        /// </summary>
        public decimal TrainingHour { get; set; }

        /// <summary>
        /// 培训级别
        /// </summary>
        public string TrainingLevel { get; set; }

        /// <summary>
        /// 培训分数
        /// </summary>
        public string TrainingScore { get; set; }

        /// <summary>
        /// 培训月份
        /// </summary>
        public int TrainingMonth { get; set; }

    }
}
