namespace ADTO.DCloud.Sessions.Dto
{
    public class GetCurrentSessionOutput
    {

        public UserLoginInfoDto User { get; set; }

        public UserLoginInfoDto ImpersonatorUser { get; set; }

        public TenantLoginInfoDto Tenant { get; set; }

        public TenantLoginInfoDto ImpersonatorTenant { get; set; }

        public ApplicationInfoDto Application { get; set; }
    }
}
