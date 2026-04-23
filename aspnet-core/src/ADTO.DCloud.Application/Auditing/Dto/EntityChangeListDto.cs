using System;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Events.Bus.Entities;

namespace ADTO.DCloud.Auditing.Dto
{
    public class EntityChangeListDto : EntityDto<long>
    {
        public Guid? UserId { get; set; }

        public string UserName { get; set; }

        public DateTime ChangeTime { get; set; }

        public string EntityTypeFullName { get; set; }

        public EntityChangeType ChangeType { get; set; }

        public string ChangeTypeName => ChangeType.ToString();

        public long EntityChangeSetId { get; set; }
    }
}