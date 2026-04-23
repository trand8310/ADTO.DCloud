
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.Localization;
using System;

namespace ADTO.DCloud.Localization.Dto
{
    [AutoMap(typeof(ApplicationLanguage))]
    public class ApplicationLanguageListDto : FullAuditedEntityDto<Guid>
    {
        public virtual Guid? TenantId { get; set; }
        
        public virtual string Name { get; set; }

        public virtual string DisplayName { get; set; }

        public virtual string Icon { get; set; }

        public bool IsDisabled { get; set; }
    }
}