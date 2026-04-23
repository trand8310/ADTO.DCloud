using System.Reflection;
using ADTOSharp.Dependency;
using ADTOSharp.HtmlSanitizer.Configuration;
using ADTOSharp.Modules;
using Ganss.Xss;

namespace ADTOSharp.HtmlSanitizer;

[DependsOn(typeof(ADTOSharpKernelModule))]
public class ADTOSharpHtmlSanitizerModule : ADTOSharpModule
{
    public override void PreInitialize()
    {
        IocManager.Register<IHtmlSanitizerConfiguration, HtmlSanitizerConfiguration>();
        IocManager.Register<IHtmlSanitizer, Ganss.Xss.HtmlSanitizer>(DependencyLifeStyle.Transient);
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
    }
}