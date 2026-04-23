

using ADTOSharp.Localization;

namespace Volo.Abp.ExceptionHandling;

public interface ILocalizeErrorMessage
{
    string LocalizeMessage(LocalizationContext context);
}
