using ADTOSharp.Dependency;
using System.Threading.Tasks;
 

namespace ADTOSharp.ExceptionHandling;

[ExposeServices(typeof(IExceptionSubscriber))]
public abstract class ExceptionSubscriber : IExceptionSubscriber, ITransientDependency
{
    public abstract Task HandleAsync(ExceptionNotificationContext context);
}
