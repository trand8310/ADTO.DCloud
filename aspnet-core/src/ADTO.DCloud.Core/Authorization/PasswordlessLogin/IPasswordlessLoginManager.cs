using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADTO.DCloud.Authorization.Users;
using ADTOSharp.Domain.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ADTO.DCloud.Authorization.PasswordlessLogin;

public interface IPasswordlessLoginManager : IDomainService
{
    Task<User> GetUserByPasswordlessProviderAndKeyAsync(string provider, string providerKey);

    Task VerifyPasswordlessLoginCode(Guid? tenantId, string providerValue, string code);
    
    Task<string> GeneratePasswordlessLoginCode(Guid? tenantId, string providerKey);
    
    Task RemovePasswordlessLoginCode(Guid? tenantId, string providerKey);
    
    List<SelectListItem> GetProviders();
}