using System;
using System.IO;
using System.Linq.Expressions;
using ADTOSharp.Application.Features;
using ADTOSharp.Application.Navigation;
using ADTOSharp.Application.Services;
using ADTOSharp.Auditing;
using ADTOSharp.Authorization;
using ADTOSharp.BackgroundJobs;
using ADTOSharp.CachedUniqueKeys;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Configuration;
using ADTOSharp.Configuration.Startup;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Uow;
using ADTOSharp.DynamicEntityProperties;
using ADTOSharp.EntityHistory;
using ADTOSharp.Events.Bus;
using ADTOSharp.Localization;
using ADTOSharp.Localization.Dictionaries;
using ADTOSharp.Localization.Dictionaries.Xml;
using ADTOSharp.Modules;
using ADTOSharp.MultiTenancy;
using ADTOSharp.Net.Mail;
using ADTOSharp.Notifications;
using ADTOSharp.RealTime;
using ADTOSharp.Reflection.Extensions;
using ADTOSharp.Runtime;
using ADTOSharp.Runtime.Caching;
using ADTOSharp.Runtime.Remoting;
using ADTOSharp.Runtime.Validation.Interception;
using ADTOSharp.Threading;
using ADTOSharp.Threading.BackgroundWorkers;
using ADTOSharp.Timing;
using ADTOSharp.Webhooks;
using Castle.MicroKernel.Registration;

namespace ADTOSharp
{
    /// <summary>
    /// ADTO 系统的核心模块。
    /// 无需依赖此模块，它始终是自动加载的第一个模块。
    /// </summary>
    public sealed class ADTOSharpKernelModule : ADTOSharpModule
    {
        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(new BasicConventionalRegistrar());

            IocManager.Register<IScopedIocResolver, ScopedIocResolver>(DependencyLifeStyle.Transient);
            IocManager.Register(typeof(IAmbientScopeProvider<>), typeof(DataContextAmbientScopeProvider<>), DependencyLifeStyle.Transient);

            AddAuditingSelectors();
            AddLocalizationSources();
            AddSettingProviders();
            AddUnitOfWorkFilters();
            AddUnitOfWorkAuditFieldConfiguration();
            ConfigureCaches();
            AddIgnoredTypes();
            AddMethodParameterValidators();
        }

        public override void Initialize()
        {
            foreach (var replaceAction in ((ADTOSharpStartupConfiguration)Configuration).ServiceReplaceActions.Values)
            {
                replaceAction();
            }

            IocManager.IocContainer.Install(new EventBusInstaller(IocManager));

            IocManager.Register(typeof(EventTriggerAsyncBackgroundJob<>), DependencyLifeStyle.Transient);

            IocManager.RegisterAssemblyByConvention(typeof(ADTOSharpKernelModule).GetAssembly(),
                new ConventionalRegistrationConfig
                {
                    InstallInstallers = false
                });

            RegisterInterceptors();
        }

        private void RegisterInterceptors()
        {
            IocManager.Register(typeof(ADTOSharpAsyncDeterminationInterceptor<UnitOfWorkInterceptor>), DependencyLifeStyle.Transient);
            IocManager.Register(typeof(ADTOSharpAsyncDeterminationInterceptor<AuditingInterceptor>), DependencyLifeStyle.Transient);
            IocManager.Register(typeof(ADTOSharpAsyncDeterminationInterceptor<AuthorizationInterceptor>), DependencyLifeStyle.Transient);
            IocManager.Register(typeof(ADTOSharpAsyncDeterminationInterceptor<ValidationInterceptor>), DependencyLifeStyle.Transient);
            IocManager.Register(typeof(ADTOSharpAsyncDeterminationInterceptor<EntityHistoryInterceptor>), DependencyLifeStyle.Transient);
        }

        public override void PostInitialize()
        {
            RegisterMissingComponents();

            IocManager.Resolve<SettingDefinitionManager>().Initialize();
            IocManager.Resolve<FeatureManager>().Initialize();
            IocManager.Resolve<PermissionManager>().Initialize();
            IocManager.Resolve<LocalizationManager>().Initialize();
            IocManager.Resolve<NotificationDefinitionManager>().Initialize();
            IocManager.Resolve<NavigationManager>().Initialize();
            IocManager.Resolve<WebhookDefinitionManager>().Initialize();
            IocManager.Resolve<DynamicEntityPropertyDefinitionManager>().Initialize();

            if (Configuration.BackgroundJobs.IsJobExecutionEnabled)
            {
                var workerManager = IocManager.Resolve<IBackgroundWorkerManager>();
                workerManager.Start();
                workerManager.Add(IocManager.Resolve<IBackgroundJobManager>());
            }
        }

        public override void Shutdown()
        {
            if (Configuration.BackgroundJobs.IsJobExecutionEnabled)
            {
                IocManager.Resolve<IBackgroundWorkerManager>().StopAndWaitToStop();
            }
        }

        private void AddUnitOfWorkFilters()
        {
            Configuration.UnitOfWork.RegisterFilter(ADTOSharpDataFilters.SoftDelete, true);
            Configuration.UnitOfWork.RegisterFilter(ADTOSharpDataFilters.MustHaveTenant, true);
            Configuration.UnitOfWork.RegisterFilter(ADTOSharpDataFilters.MayHaveTenant, true);
        }

        private void AddUnitOfWorkAuditFieldConfiguration()
        {
            Configuration.UnitOfWork.RegisterAuditFieldConfiguration(ADTOSharpAuditFields.CreatorUserId, true);
            Configuration.UnitOfWork.RegisterAuditFieldConfiguration(ADTOSharpAuditFields.LastModifierUserId, true);
            Configuration.UnitOfWork.RegisterAuditFieldConfiguration(ADTOSharpAuditFields.LastModificationTime, true);
            Configuration.UnitOfWork.RegisterAuditFieldConfiguration(ADTOSharpAuditFields.DeleterUserId, true);
            Configuration.UnitOfWork.RegisterAuditFieldConfiguration(ADTOSharpAuditFields.DeletionTime, true);
        }

        private void AddSettingProviders()
        {
            Configuration.Settings.Providers.Add<LocalizationSettingProvider>();
            Configuration.Settings.Providers.Add<EmailSettingProvider>();
            Configuration.Settings.Providers.Add<NotificationSettingProvider>();
            Configuration.Settings.Providers.Add<TimingSettingProvider>();
        }

        private void AddAuditingSelectors()
        {
            Configuration.Auditing.Selectors.Add(
                new NamedTypeSelector(
                    "ADTOSharp.ApplicationServices",
                    type => typeof(IApplicationService).IsAssignableFrom(type)
                )
            );
        }

        private void AddLocalizationSources()
        {
            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    ADTOSharpConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(ADTOSharpKernelModule).GetAssembly(), "ADTOSharp.Localization.Sources.ADTOSharpXmlSource"
                    )));
        }

        private void ConfigureCaches()
        {
            Configuration.Caching.Configure(ADTOSharpCacheNames.ApplicationSettings, cache =>
            {
                cache.DefaultSlidingExpireTime = TimeSpan.FromHours(8);
            });

            Configuration.Caching.Configure(ADTOSharpCacheNames.TenantSettings, cache =>
            {
                cache.DefaultSlidingExpireTime = TimeSpan.FromMinutes(60);
            });

            Configuration.Caching.Configure(ADTOSharpCacheNames.UserSettings, cache =>
            {
                cache.DefaultSlidingExpireTime = TimeSpan.FromMinutes(20);
            });
        }

        private void AddIgnoredTypes()
        {
            var commonIgnoredTypes = new[]
            {
                typeof(Stream),
                typeof(Expression)
            };

            foreach (var ignoredType in commonIgnoredTypes)
            {
                Configuration.Auditing.IgnoredTypes.AddIfNotContains(ignoredType);
                Configuration.Validation.IgnoredTypes.AddIfNotContains(ignoredType);
            }

            var validationIgnoredTypes = new[] { typeof(Type) };
            foreach (var ignoredType in validationIgnoredTypes)
            {
                Configuration.Validation.IgnoredTypes.AddIfNotContains(ignoredType);
            }
        }

        private void AddMethodParameterValidators()
        {
            Configuration.Validation.Validators.Add<DataAnnotationsValidator>();
            Configuration.Validation.Validators.Add<ValidatableObjectValidator>();
            Configuration.Validation.Validators.Add<CustomValidator>();
        }

        private void RegisterMissingComponents()
        {
            if (!IocManager.IsRegistered<IGuidGenerator>())
            {
                IocManager.IocContainer.Register(
                    Component
                        .For<IGuidGenerator, SequentialGuidGenerator>()
                        .Instance(SequentialGuidGenerator.Instance)
                );
            }

            IocManager.RegisterIfNot<IUnitOfWork, NullUnitOfWork>(DependencyLifeStyle.Transient);
            IocManager.RegisterIfNot<IAuditingStore, SimpleLogAuditingStore>(DependencyLifeStyle.Singleton);
            IocManager.RegisterIfNot<IPermissionChecker, NullPermissionChecker>(DependencyLifeStyle.Singleton);
            IocManager.RegisterIfNot<INotificationStore, NullNotificationStore>(DependencyLifeStyle.Singleton);
            IocManager.RegisterIfNot<IUnitOfWorkFilterExecuter, NullUnitOfWorkFilterExecuter>(DependencyLifeStyle.Singleton);
            IocManager.RegisterIfNot<IClientInfoProvider, NullClientInfoProvider>(DependencyLifeStyle.Singleton);
            IocManager.RegisterIfNot<ITenantStore, NullTenantStore>(DependencyLifeStyle.Singleton);
            IocManager.RegisterIfNot<ITenantResolverCache, NullTenantResolverCache>(DependencyLifeStyle.Singleton);
            IocManager.RegisterIfNot<IEntityHistoryStore, NullEntityHistoryStore>(DependencyLifeStyle.Singleton);
            IocManager.RegisterIfNot<ICachedUniqueKeyPerUser, CachedUniqueKeyPerUser>(DependencyLifeStyle.Transient);

            if (Configuration.BackgroundJobs.IsJobExecutionEnabled)
            {
                IocManager.RegisterIfNot<IBackgroundJobStore, InMemoryBackgroundJobStore>(DependencyLifeStyle.Singleton);
            }
            else
            {
                IocManager.RegisterIfNot<IBackgroundJobStore, NullBackgroundJobStore>(DependencyLifeStyle.Singleton);
            }
        }
    }
}
