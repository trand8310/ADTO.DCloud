using ADTOSharp.Dependency;

namespace ADTO.DCloud.Web.Xss;

public interface IHtmlSanitizer: ITransientDependency
{
    string Sanitize(string html);
}