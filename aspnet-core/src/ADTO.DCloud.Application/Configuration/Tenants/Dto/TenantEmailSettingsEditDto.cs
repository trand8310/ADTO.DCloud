
using ADTO.DCloud.Configuration.Dto;

namespace ADTO.DCloud.Configuration.Tenants.Dto
{
    public class TenantEmailSettingsEditDto : EmailSettingsEditDto
    {
        public bool UseHostDefaultEmailSettings { get; set; }
    }
}