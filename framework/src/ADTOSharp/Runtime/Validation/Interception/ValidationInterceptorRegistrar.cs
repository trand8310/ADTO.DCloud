using System.Reflection;
using ADTOSharp.Dependency;
using Castle.Core;

namespace ADTOSharp.Runtime.Validation.Interception
{
    internal static class ValidationInterceptorRegistrar
    {
        public static void Initialize(IIocManager iocManager)
        {
            iocManager.IocContainer.Kernel.ComponentRegistered += (key, handler) =>
            {
                var implementationType = handler.ComponentModel.Implementation.GetTypeInfo();
            
                if (!iocManager.IsRegistered<IADTOSharpValidationDefaultOptions>())
                {
                    return;
                }
                
                var validationOptions = iocManager.Resolve<IADTOSharpValidationDefaultOptions>();

                if (validationOptions.IsConventionalValidationClass(implementationType.AsType()))
                {
                    handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(ADTOSharpAsyncDeterminationInterceptor<ValidationInterceptor>)));
                }
            };
        }
    }
}
