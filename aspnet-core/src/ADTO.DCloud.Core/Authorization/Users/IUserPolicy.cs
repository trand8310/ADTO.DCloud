using System;
using System.Threading.Tasks;
using ADTOSharp.Domain.Policies;

namespace ADTO.DCloud.Authorization.Users
{
    public interface IUserPolicy : IPolicy
    {
        Task CheckMaxUserCountAsync(Guid tenantId);
    }
}
