using System;
using System.Reflection;
using ADTOSharp.Auditing;
using ADTOSharp.Authorization;
using ADTOSharp.Configuration.Startup;
using ADTOSharp.Dependency;
using ADTOSharp.Dependency.Installers;
using ADTOSharp.Domain.Uow;
using ADTOSharp.EntityHistory;
using ADTOSharp.Modules;
using ADTOSharp.PlugIns;
using ADTOSharp.Runtime.Validation.Interception;
using Castle.Core.Logging;
using Castle.MicroKernel.Registration;
using JetBrains.Annotations;

namespace ADTOSharp
{
    /// <summary>
    ///这是负责启动整个 ADTO 系统的主要类。
    ///准备依赖注入并注册启动所需的核心组件。
    ///在应用程序中，必须首先实例化并初始化它（请参阅 <see cref="Initialize"/>）。
    /// </summary>
    public class ADTOSharpBootstrapper : IDisposable
    {
        /// <summary>
        /// Get the startup module of the application which depends on other used modules.
        /// </summary>
        public Type StartupModule { get; }

        /// <summary>
        /// A list of plug in folders.
        /// </summary>
        public PlugInSourceList PlugInSources { get; }

        /// <summary>
        /// Gets IIocManager object used by this class.
        /// </summary>
        public IIocManager IocManager { get; }

        /// <summary>
        /// Is this object disposed before?
        /// </summary>
        protected bool IsDisposed;

        private ADTOSharpModuleManager _moduleManager;
        private ILogger _logger;

        /// <summary>
        /// Creates a new <see cref="ADTOSharpBootstrapper"/> instance.
        /// </summary>
        /// <param name="startupModule">Startup module of the application which depends on other used modules. Should be derived from <see cref="ADTOSharpModule"/>.</param>
        /// <param name="optionsAction">An action to set options</param>
        private ADTOSharpBootstrapper(
            [NotNull] Type startupModule, 
            [CanBeNull] Action<ADTOSharpBootstrapperOptions> optionsAction = null)
        {
            Check.NotNull(startupModule, nameof(startupModule));

            var options = new ADTOSharpBootstrapperOptions();
            optionsAction?.Invoke(options);

            if (!typeof(ADTOSharpModule).GetTypeInfo().IsAssignableFrom(startupModule))
            {
                throw new ArgumentException($"{nameof(startupModule)} should be derived from {nameof(ADTOSharpModule)}.");
            }

            StartupModule = startupModule;

            IocManager = options.IocManager;
            PlugInSources = options.PlugInSources;

            _logger = NullLogger.Instance;
            
            AddInterceptorRegistrars(options.InterceptorOptions);
        }

        /// <summary>
        /// Creates a new <see cref="ADTOSharpBootstrapper"/> instance.
        /// </summary>
        /// <typeparam name="TStartupModule">Startup module of the application which depends on other used modules. Should be derived from <see cref="ADTOSharpModule"/>.</typeparam>
        /// <param name="optionsAction">An action to set options</param>
        public static ADTOSharpBootstrapper Create<TStartupModule>(
            [CanBeNull] Action<ADTOSharpBootstrapperOptions> optionsAction = null)
            where TStartupModule : ADTOSharpModule
        {
            return new ADTOSharpBootstrapper(typeof(TStartupModule), optionsAction);
        }

        /// <summary>
        /// Creates a new <see cref="ADTOSharpBootstrapper"/> instance.
        /// </summary>
        /// <param name="startupModule">Startup module of the application which depends on other used modules. Should be derived from <see cref="ADTOSharpModule"/>.</param>
        /// <param name="optionsAction">An action to set options</param>
        public static ADTOSharpBootstrapper Create(
            [NotNull] Type startupModule, 
            [CanBeNull] Action<ADTOSharpBootstrapperOptions> optionsAction = null)
        {
            return new ADTOSharpBootstrapper(startupModule, optionsAction);
        }

        private void AddInterceptorRegistrars(
            ADTOSharpBootstrapperInterceptorOptions options)
        {
            if (!options.DisableValidationInterceptor)
            {
                ValidationInterceptorRegistrar.Initialize(IocManager);    
            }

            if (!options.DisableAuditingInterceptor)
            {
                AuditingInterceptorRegistrar.Initialize(IocManager);
            }

            if (!options.DisableEntityHistoryInterceptor)
            {
                EntityHistoryInterceptorRegistrar.Initialize(IocManager);    
            }

            if (!options.DisableUnitOfWorkInterceptor)
            {
                UnitOfWorkRegistrar.Initialize(IocManager);
            }

            if (!options.DisableAuthorizationInterceptor)
            {
                AuthorizationInterceptorRegistrar.Initialize(IocManager);   
            }
        }

        /// <summary>
        /// Initializes the ADTO system.
        /// </summary>
        public virtual void Initialize()
        {
            ResolveLogger();

            try
            {
                RegisterBootstrapper();
                IocManager.IocContainer.Install(new ADTOSharpCoreInstaller());

                IocManager.Resolve<ADTOSharpPlugInManager>().PlugInSources.AddRange(PlugInSources);
                IocManager.Resolve<ADTOSharpStartupConfiguration>().Initialize();

                _moduleManager = IocManager.Resolve<ADTOSharpModuleManager>();
                _moduleManager.Initialize(StartupModule);
                _moduleManager.StartModules();
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.ToString(), ex);
                throw;
            }
        }

        private void ResolveLogger()
        {
            if (IocManager.IsRegistered<ILoggerFactory>())
            {
                _logger = IocManager.Resolve<ILoggerFactory>().Create(typeof(ADTOSharpBootstrapper));
            }
        }

        private void RegisterBootstrapper()
        {
            if (!IocManager.IsRegistered<ADTOSharpBootstrapper>())
            {
                IocManager.IocContainer.Register(
                    Component.For<ADTOSharpBootstrapper>().Instance(this)
                );
            }
        }

        /// <summary>
        /// Disposes the ADTO system.
        /// </summary>
        public virtual void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            _moduleManager?.ShutdownModules();
        }
    }
}
