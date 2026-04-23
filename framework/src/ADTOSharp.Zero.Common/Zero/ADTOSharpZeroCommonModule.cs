using System.Linq;
using System.Reflection;
using ADTOSharp.Application.Features;
using ADTOSharp.Auditing;
using ADTOSharp.Authorization.Roles;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Configuration.Startup;
using ADTOSharp.Dependency;
using ADTOSharp.Localization;
using ADTOSharp.Localization.Dictionaries;
using ADTOSharp.Localization.Dictionaries.Xml;
using ADTOSharp.Modules;
using ADTOSharp.MultiTenancy;
using ADTOSharp.Reflection;
using ADTOSharp.Reflection.Extensions;
using ADTOSharp.Zero.Configuration;
using Castle.MicroKernel.Registration;

namespace ADTOSharp.Zero
{
    /// <summary>
    /// ADTO zero core module.
    /// </summary>
    [DependsOn(typeof(ADTOSharpKernelModule))]
    public class ADTOSharpZeroCommonModule : ADTOSharpModule
    {
        public override void PreInitialize()
        {
            IocManager.RegisterIfNot<IADTOSharpZeroEntityTypes, ADTOSharpZeroEntityTypes>(); //Registered on services.AddADTOSharpIdentity() for ADTOSharp.ZeroCore.

            IocManager.Register<IRoleManagementConfig, RoleManagementConfig>();
            IocManager.Register<IUserManagementConfig, UserManagementConfig>();
            IocManager.Register<ILanguageManagementConfig, LanguageManagementConfig>();
            IocManager.Register<IADTOSharpZeroConfig, ADTOSharpZeroConfig>();

            Configuration.ReplaceService<ITenantStore, TenantStore>(DependencyLifeStyle.Transient);

            Configuration.Settings.Providers.Add<ADTOSharpZeroSettingProvider>();

            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    ADTOSharpZeroConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(ADTOSharpZeroCommonModule).GetAssembly(), "ADTOSharp.Zero.Localization.Source"
                        )));

            IocManager.IocContainer.Kernel.ComponentRegistered += Kernel_ComponentRegistered;

            AddIgnoredTypes();
        }

        public override void Initialize()
        {
            FillMissingEntityTypes();

            IocManager.Register<IMultiTenantLocalizationDictionary, MultiTenantLocalizationDictionary>(DependencyLifeStyle.Transient);
            IocManager.RegisterAssemblyByConvention(typeof(ADTOSharpZeroCommonModule).GetAssembly());

            RegisterTenantCache();
        }

        private void Kernel_ComponentRegistered(string key, Castle.MicroKernel.IHandler handler)
        {
            if (typeof(IADTOSharpZeroFeatureValueStore).IsAssignableFrom(handler.ComponentModel.Implementation) && !IocManager.IsRegistered<IADTOSharpZeroFeatureValueStore>())
            {
                IocManager.IocContainer.Register(
                    Component.For<IADTOSharpZeroFeatureValueStore>().ImplementedBy(handler.ComponentModel.Implementation).Named("ADTOSharpZeroFeatureValueStore").LifestyleTransient()
                    );
            }
        }

        private void AddIgnoredTypes()
        {
            var ignoredTypes = new[]
            {
                typeof(AuditLog)
            };

            foreach (var ignoredType in ignoredTypes)
            {
                Configuration.EntityHistory.IgnoredTypes.AddIfNotContains(ignoredType);
            }
        }

        private void FillMissingEntityTypes()
        {
            using (var entityTypes = IocManager.ResolveAsDisposable<IADTOSharpZeroEntityTypes>())
            {
                if (entityTypes.Object.User != null &&
                    entityTypes.Object.Role != null &&
                    entityTypes.Object.Tenant != null)
                {
                    return;
                }

                using (var typeFinder = IocManager.ResolveAsDisposable<ITypeFinder>())
                {
                    var types = typeFinder.Object.FindAll();
                    entityTypes.Object.Tenant = types.FirstOrDefault(t => typeof(ADTOSharpTenantBase).IsAssignableFrom(t) && !t.GetTypeInfo().IsAbstract);
                    entityTypes.Object.Role = types.FirstOrDefault(t => typeof(ADTOSharpRoleBase).IsAssignableFrom(t) && !t.GetTypeInfo().IsAbstract);
                    entityTypes.Object.User = types.FirstOrDefault(t => typeof(ADTOSharpUserBase).IsAssignableFrom(t) && !t.GetTypeInfo().IsAbstract);
                }
            }
        }

        private void RegisterTenantCache()
        {
            if (IocManager.IsRegistered<ITenantCache>())
            {
                return;
            }

            using (var entityTypes = IocManager.ResolveAsDisposable<IADTOSharpZeroEntityTypes>())
            {
                var implType = typeof (TenantCache<,>)
                    .MakeGenericType(entityTypes.Object.Tenant, entityTypes.Object.User);

                IocManager.Register(typeof (ITenantCache), implType, DependencyLifeStyle.Transient);
            }
        }
    }
}
