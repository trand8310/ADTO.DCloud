using System;
using System.Linq.Expressions;
using ADTOSharp.Specifications;
using ADTOSharp.Timing;

namespace ADTO.DCloud.Authorization.Delegation
{
    /// <summary>
    /// 用户委托,用于实现在指定的时间内可以行使被委托人的功能
    /// </summary>
    public class ActiveUserDelegationSpecification : Specification<UserDelegation>
    {
        /// <summary>
        /// 来源用户Id,被委托人
        /// </summary>
        public Guid SourceUserId { get; }
        /// <summary>
        /// 目标用户ID,委托人
        /// </summary>
        public Guid TargetUserId { get; }

        public ActiveUserDelegationSpecification(Guid sourceUserId, Guid targetUserId)
        {
            SourceUserId = sourceUserId;
            TargetUserId = targetUserId;
        }

        public override Expression<Func<UserDelegation, bool>> ToExpression()
        {
            var now = Clock.Now;
            return (e) => (e.SourceUserId == SourceUserId &&
                           e.TargetUserId == TargetUserId &&
                           e.StartTime <= now && e.EndTime >= now);
        }
    }
}