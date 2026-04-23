using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADTOSharp;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Domain.Repositories;

namespace ADTO.DCloud.Organizations
{
    public interface IUserOrganizationUnitRepository : IRepository<UserOrganizationUnit, Guid>
    {
        Task<List<UserIdentifier>> GetAllUsersInOrganizationUnitHierarchical(Guid[] organizationUnitIds);
    }
}