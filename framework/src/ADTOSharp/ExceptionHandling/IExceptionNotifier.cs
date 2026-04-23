using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ADTOSharp.ExceptionHandling;

public interface IExceptionNotifier
{
    Task NotifyAsync([NotNull] ExceptionNotificationContext context);
}
