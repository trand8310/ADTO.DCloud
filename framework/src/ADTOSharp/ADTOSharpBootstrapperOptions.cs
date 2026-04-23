using ADTOSharp.Dependency;
using ADTOSharp.PlugIns;

namespace ADTOSharp
{
    public class ADTOSharpBootstrapperOptions
    {
        /// <summary>
        /// Used to disable all interceptors added by ADTO.
        /// </summary>
        public ADTOSharpBootstrapperInterceptorOptions InterceptorOptions { get; set; }

        /// <summary>
        /// IIocManager that is used to bootstrap the ADTO system. If set to null, uses global <see cref="ADTOSharp.Dependency.IocManager.Instance"/>
        /// </summary>
        public IIocManager IocManager { get; set; }

        /// <summary>
        /// List of plugin sources.
        /// </summary>
        public PlugInSourceList PlugInSources { get; }

        public ADTOSharpBootstrapperOptions()
        {
            IocManager = ADTOSharp.Dependency.IocManager.Instance;
            PlugInSources = new PlugInSourceList();
            InterceptorOptions = new ADTOSharpBootstrapperInterceptorOptions();
        }
    }

    public class ADTOSharpBootstrapperInterceptorOptions
    {
        public bool DisableValidationInterceptor { get; set; }
        
        public bool DisableAuditingInterceptor { get; set; }
        
        public bool DisableEntityHistoryInterceptor { get; set; }
        
        public bool DisableUnitOfWorkInterceptor { get; set; }
        
        public bool DisableAuthorizationInterceptor { get; set; }
    }
}
