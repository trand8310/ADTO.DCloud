using ADTOSharp.AspNetCore.SignalR.Hubs;
using ADTOSharp.Runtime.Security;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ADTO.DCloud.Chat.SignalR
{
    public abstract class TenantHubBase : ADTOSharpHubBase
    {
        
        protected string TenantGroup =>
            ADTOSharpSession.TenantId.HasValue
                ? $"tenant:{ADTOSharpSession.TenantId}"
                : "tenant:host";

        public override async Task OnConnectedAsync()
        {
            //var tenantId = Context.User.Claims.FirstOrDefault(c => c.Type == ADTOSharpClaimTypes.TenantId);


            await Groups.AddToGroupAsync(Context.ConnectionId, TenantGroup);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, TenantGroup);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
