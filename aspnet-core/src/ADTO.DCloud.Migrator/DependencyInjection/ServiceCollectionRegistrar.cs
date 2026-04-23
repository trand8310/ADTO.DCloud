using Microsoft.Extensions.DependencyInjection;
using Castle.Windsor.MsDependencyInjection;
using ADTOSharp.Dependency;
using ADTO.DCloud.Identity;

namespace ADTO.DCloud.Migrator.DependencyInjection
{
    public static class ServiceCollectionRegistrar
    {
        public static void Register(IIocManager iocManager)
        {
            var services = new ServiceCollection();

            IdentityRegistrar.Register(services);

            WindsorRegistrationHelper.CreateServiceProvider(iocManager.IocContainer, services);
        }
    }
}
