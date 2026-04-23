using ADTOSharp.MultiTenancy;

namespace ADTOSharp.Web.Models.ADTOSharpUserConfiguration
{
    public class ADTOSharpMultiTenancySidesConfigDto
    {
        public MultiTenancySides Host { get; private set; }

        public MultiTenancySides Tenant { get; private set; }

        public ADTOSharpMultiTenancySidesConfigDto()
        {
            Host = MultiTenancySides.Host;
            Tenant = MultiTenancySides.Tenant;
        }
    }
}