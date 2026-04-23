using ADTOSharp.Domain.Uow;
using System.Threading.Tasks;

namespace ADTO.OpenIddict;

public interface IOpenIddictDbConcurrencyExceptionHandler
{
    Task HandleAsync(ADTOSharpDbConcurrencyException exception);
}