using ADTOSharp.Dependency;
using ADTOSharp.Configuration.Startup;
using ADTOSharp.Modules;
using ADTOSharp.Net.Mail;
using ADTOSharp.Reflection.Extensions;

namespace ADTOSharp.MailKit
{
    [DependsOn(typeof(ADTOSharpKernelModule))]
    public class ADTOSharpMailKitModule : ADTOSharpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IADTOSharpMailKitConfiguration, ADTOSharpMailKitConfiguration>();
            Configuration.ReplaceService<IEmailSender, MailKitEmailSender>(DependencyLifeStyle.Transient);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ADTOSharpMailKitModule).GetAssembly());
        }
    }
}
