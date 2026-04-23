namespace ADTOSharp.Web.Models.ADTOSharpUserConfiguration
{
    public class ADTOSharpMultiTenancyConfigDto
    {
        public bool IsEnabled { get; set; }

        public bool IgnoreFeatureCheckForHostUsers { get; set; }

        public ADTOSharpMultiTenancySidesConfigDto Sides { get; private set; }

        public ADTOSharpMultiTenancyConfigDto()
        {
            Sides = new ADTOSharpMultiTenancySidesConfigDto();
        }
    }
}