using ADTO.OpenIddict.Applications;
using ADTO.OpenIddict.Authorizations;
using ADTO.OpenIddict.Scopes;
using ADTO.OpenIddict.Tokens;
using Microsoft.EntityFrameworkCore;

namespace ADTO.OpenIddict.EntityFrameworkCore;

public interface IOpenIddictDbContext
{
    DbSet<OpenIddictApplication> Applications { get; }

    DbSet<OpenIddictAuthorization> Authorizations { get; }

    DbSet<OpenIddictScope> Scopes { get; }

    DbSet<OpenIddictToken> Tokens { get; }
}