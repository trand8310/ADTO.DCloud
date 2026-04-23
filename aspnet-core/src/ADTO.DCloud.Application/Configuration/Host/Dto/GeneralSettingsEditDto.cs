namespace ADTO.DCloud.Configuration.Host.Dto
{
    public class GeneralSettingsEditDto
    {
        public string Timezone { get; set; }

        /// <summary>
        /// 用于比较用户的时区与默认时区
        /// </summary>
        public string TimezoneForComparison { get; set; }
    }
}