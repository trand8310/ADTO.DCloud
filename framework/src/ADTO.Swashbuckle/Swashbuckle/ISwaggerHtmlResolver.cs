using System.IO;

namespace ADTO.Swashbuckle;

public interface ISwaggerHtmlResolver
{
    Stream Resolver();
}
