using System.IO;
using System.Reflection;
using System.Text;
using ADTOSharp.Dependency;
using Swashbuckle.AspNetCore.SwaggerUI;


namespace ADTO.Swashbuckle;

public class SwaggerHtmlResolver : ISwaggerHtmlResolver, ITransientDependency
{
    public virtual Stream Resolver()
    {
        var scriptBundleScript = "<script src=\"%(ScriptBundlePath)\" charset=\"utf-8\"></script>";
        var swaggerScript = "<script src=\"ui/adto.swagger.js\" charset=\"utf-8\"></script>";
        var stream = typeof(SwaggerUIOptions).GetTypeInfo().Assembly
            .GetManifestResourceStream("Swashbuckle.AspNetCore.SwaggerUI.index.html");

        var html = new StreamReader(stream!)
            .ReadToEnd()
            .Replace(scriptBundleScript, $"{scriptBundleScript}\n{swaggerScript}");

        return new MemoryStream(Encoding.UTF8.GetBytes(html));
    }
}
