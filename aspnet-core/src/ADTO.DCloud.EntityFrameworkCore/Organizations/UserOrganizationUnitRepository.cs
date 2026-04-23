using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTOSharp;
using ADTOSharp.Authorization.Users;
using ADTOSharp.EntityFrameworkCore;
using ADTOSharp.Organizations;
using ADTOSharp.UI;
using Microsoft.EntityFrameworkCore;
using ADTO.DCloud.EntityFrameworkCore;
using ADTO.DCloud.EntityFrameworkCore.Repositories;
using System;
using ADTOSharp.Linq.Expressions;

namespace ADTO.DCloud.Organizations;

public class UserOrganizationUnitRepository : DCloudRepositoryBase<UserOrganizationUnit, Guid>,
    IUserOrganizationUnitRepository
{
    public UserOrganizationUnitRepository(IDbContextProvider<DCloudDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<UserIdentifier>> GetAllUsersInOrganizationUnitHierarchical(Guid[] organizationUnitIds)
    {
        if (organizationUnitIds == null || organizationUnitIds.Length == 0)
        {
            return new List<UserIdentifier>();
        }

        var context = await GetContextAsync();

        var selectedOrganizationUnitCodes = await context.OrganizationUnits
            .Where(ou => organizationUnitIds.Contains(ou.Id))
            .ToListAsync();

        if (selectedOrganizationUnitCodes == null)
        {
            throw new UserFriendlyException("Can not find an organization unit");
        }

        var predicate = PredicateBuilder.New<OrganizationUnit>();

        foreach (var selectedOrganizationUnitCode in selectedOrganizationUnitCodes)
        {
            predicate = predicate.Or(ou => ou.Code.StartsWith(selectedOrganizationUnitCode.Code));
        }

        var userIdQueryHierarchical = await context.UserOrganizationUnits
            .Join(
                context.OrganizationUnits.Where(predicate),
                uo => uo.OrganizationUnitId,
                ou => ou.Id,
                (uo, ou) => new { uo.UserId, uo.TenantId }
            )
            .ToListAsync();

        return userIdQueryHierarchical
            .DistinctBy(x => x.UserId)
            .Select(ou => new UserIdentifier(ou.TenantId, ou.UserId))
            .ToList();
    }
}
