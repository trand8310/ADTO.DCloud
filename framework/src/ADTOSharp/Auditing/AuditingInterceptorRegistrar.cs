using System;
using System.Linq;
using System.Reflection;
using ADTOSharp.Dependency;
using Castle.Core;

namespace ADTOSharp.Auditing
{
    internal static class AuditingInterceptorRegistrar
    {
        public static void Initialize(IIocManager iocManager)
        {
            iocManager.IocContainer.Kernel.ComponentRegistered += (key, handler) =>
            {
                if (ShouldIntercept(iocManager, handler.ComponentModel.Implementation))
                {
                    handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(ADTOSharpAsyncDeterminationInterceptor<AuditingInterceptor>)));
                }
            };
        }
        
        private static bool ShouldIntercept(IIocManager iocManager, Type type)
        {
            if (type.GetTypeInfo().IsDefined(typeof(AuditedAttribute), true))
            {
                return true;
            }

            if (type.GetMethods().Any(m => m.IsDefined(typeof(AuditedAttribute), true)))
            {
                return true;
            }

            if (!iocManager.IsRegistered<IADTOSharpAuditingDefaultOptions>())
            {
                return false;
            }
            
            var auditingOptions = iocManager.Resolve<IADTOSharpAuditingDefaultOptions>();
            
            if (auditingOptions.ConventionalAuditingSelectors.Any(selector => selector(type)))
            {
                return true;
            }
            
            return false;
        }
    }
}
