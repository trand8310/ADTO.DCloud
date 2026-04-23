using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADTOSharp.Webhooks
{
    /// <summary>
    /// Store created web hooks. To see who get that webhook check with <see cref="WebhookSendAttempt.WebhookEventId"/> and you can get <see cref="WebhookSendAttempt.WebhookSubscriptionId"/>
    /// </summary>
    [Table("ADTOSharpWebhookEvents")]
    public class WebhookEvent : Entity<Guid>, IMayHaveTenant, IHasCreationTime, IHasDeletionTime
    {
        /// <summary>
        /// Webhook unique name <see cref="WebhookDefinition.Name"/>
        /// </summary>
        [Required]
        public virtual string WebhookName { get; set; }

        /// <summary>
        /// Webhook data as JSON string.
        /// </summary>
        public virtual string Data { get; set; }

        public virtual DateTime CreationTime { get; set; }

        public virtual Guid? TenantId { get; set; }

        public virtual bool IsDeleted { get; set; }

        public virtual DateTime? DeletionTime { get; set; }
    }
}
