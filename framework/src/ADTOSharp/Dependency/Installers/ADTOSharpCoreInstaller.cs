using ADTOSharp.Application.Features;
using ADTOSharp.Auditing;
using ADTOSharp.BackgroundJobs;
using ADTOSharp.Configuration.Startup;
using ADTOSharp.Domain.Uow;
using ADTOSharp.DynamicEntityProperties;
using ADTOSharp.EntityHistory;
using ADTOSharp.Localization;
using ADTOSharp.Modules;
using ADTOSharp.Notifications;
using ADTOSharp.PlugIns;
using ADTOSharp.Reflection;
using ADTOSharp.Resources.Embedded;
using ADTOSharp.Runtime.Caching.Configuration;
using ADTOSharp.Runtime.Validation;
using ADTOSharp.Webhooks;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace ADTOSharp.Dependency.Installers
{
    internal class ADTOSharpCoreInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IUnitOfWorkDefaultOptions, UnitOfWorkDefaultOptions>().ImplementedBy<UnitOfWorkDefaultOptions>().LifestyleSingleton(),
                Component.For<IADTOSharpValidationDefaultOptions, ADTOSharpValidationDefaultOptions>().ImplementedBy<ADTOSharpValidationDefaultOptions>().LifestyleSingleton(),
                Component.For<IADTOSharpAuditingDefaultOptions, ADTOSharpAuditingDefaultOptions>().ImplementedBy<ADTOSharpAuditingDefaultOptions>().LifestyleSingleton(),
                Component.For<INavigationConfiguration, NavigationConfiguration>().ImplementedBy<NavigationConfiguration>().LifestyleSingleton(),
                Component.For<ILocalizationConfiguration, LocalizationConfiguration>().ImplementedBy<LocalizationConfiguration>().LifestyleSingleton(),
                Component.For<IAuthorizationConfiguration, AuthorizationConfiguration>().ImplementedBy<AuthorizationConfiguration>().LifestyleSingleton(),
                Component.For<IValidationConfiguration, ValidationConfiguration>().ImplementedBy<ValidationConfiguration>().LifestyleSingleton(),
                Component.For<IFeatureConfiguration, FeatureConfiguration>().ImplementedBy<FeatureConfiguration>().LifestyleSingleton(),
                Component.For<ISettingsConfiguration, SettingsConfiguration>().ImplementedBy<SettingsConfiguration>().LifestyleSingleton(),
                Component.For<IModuleConfigurations, ModuleConfigurations>().ImplementedBy<ModuleConfigurations>().LifestyleSingleton(),
                Component.For<IEventBusConfiguration, EventBusConfiguration>().ImplementedBy<EventBusConfiguration>().LifestyleSingleton(),
                Component.For<IMultiTenancyConfig, MultiTenancyConfig>().ImplementedBy<MultiTenancyConfig>().LifestyleSingleton(),
                Component.For<ICachingConfiguration, CachingConfiguration>().ImplementedBy<CachingConfiguration>().LifestyleSingleton(),
                Component.For<IAuditingConfiguration, AuditingConfiguration>().ImplementedBy<AuditingConfiguration>().LifestyleSingleton(),
                Component.For<IBackgroundJobConfiguration, BackgroundJobConfiguration>().ImplementedBy<BackgroundJobConfiguration>().LifestyleSingleton(),
                Component.For<INotificationConfiguration, NotificationConfiguration>().ImplementedBy<NotificationConfiguration>().LifestyleSingleton(),
                Component.For<IEmbeddedResourcesConfiguration, EmbeddedResourcesConfiguration>().ImplementedBy<EmbeddedResourcesConfiguration>().LifestyleSingleton(),
                Component.For<IADTOSharpStartupConfiguration, ADTOSharpStartupConfiguration>().ImplementedBy<ADTOSharpStartupConfiguration>().LifestyleSingleton(),
                Component.For<IEntityHistoryConfiguration, EntityHistoryConfiguration>().ImplementedBy<EntityHistoryConfiguration>().LifestyleSingleton(),
                Component.For<ITypeFinder, TypeFinder>().ImplementedBy<TypeFinder>().LifestyleSingleton(),
                Component.For<IADTOSharpPlugInManager, ADTOSharpPlugInManager>().ImplementedBy<ADTOSharpPlugInManager>().LifestyleSingleton(),
                Component.For<IADTOSharpModuleManager, ADTOSharpModuleManager>().ImplementedBy<ADTOSharpModuleManager>().LifestyleSingleton(),
                Component.For<IAssemblyFinder, ADTOSharpAssemblyFinder>().ImplementedBy<ADTOSharpAssemblyFinder>().LifestyleSingleton(),
                Component.For<ILocalizationManager, LocalizationManager>().ImplementedBy<LocalizationManager>().LifestyleSingleton(),
                Component.For<IWebhooksConfiguration, WebhooksConfiguration>().ImplementedBy<WebhooksConfiguration>().LifestyleSingleton(),
                Component.For<IDynamicEntityPropertyDefinitionContext, DynamicEntityPropertyDefinitionContext>().ImplementedBy<DynamicEntityPropertyDefinitionContext>().LifestyleTransient(),
                Component.For<IDynamicEntityPropertyConfiguration, DynamicEntityPropertyConfiguration>().ImplementedBy<DynamicEntityPropertyConfiguration>().LifestyleSingleton()
            );
        }
    }
}
