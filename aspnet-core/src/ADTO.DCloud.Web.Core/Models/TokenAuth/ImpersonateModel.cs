using System;


namespace ADTO.DCloud.Web.Models.TokenAuth;

public class ImpersonateModel
{
    public Guid? TenantId { get; set; }
    public Guid UserId { get; set; }
}