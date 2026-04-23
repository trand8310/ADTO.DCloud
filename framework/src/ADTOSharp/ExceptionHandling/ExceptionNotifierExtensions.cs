using System;
using System.Threading.Tasks;
using ADTOSharp.Logging;
using JetBrains.Annotations;


namespace ADTOSharp.ExceptionHandling;

public static class ExceptionNotifierExtensions
{
    public static Task NotifyAsync(
        [NotNull] this IExceptionNotifier exceptionNotifier,
        [NotNull] Exception exception,
        LogSeverity? logLevel = null,
        bool handled = true)
    {
        Check.NotNull(exceptionNotifier, nameof(exceptionNotifier));

        return exceptionNotifier.NotifyAsync(
            new ExceptionNotificationContext(
                exception,
                logLevel,
                handled
            )
        );
    }
}
