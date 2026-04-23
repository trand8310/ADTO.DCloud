using System.Threading.Tasks;
using ADTOSharp.Aspects;
using ADTOSharp.Dependency;
using Castle.DynamicProxy;

namespace ADTOSharp.Runtime.Validation.Interception
{
    /// <summary>
    /// This interceptor is used intercept method calls for classes which's methods must be validated.
    /// </summary>
    public class ValidationInterceptor : ADTOSharpInterceptorBase, ITransientDependency
    {
        private readonly IIocResolver _iocResolver;

        public ValidationInterceptor(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        public override void InterceptSynchronous(IInvocation invocation)
        {
            if (ADTOSharpCrossCuttingConcerns.IsApplied(invocation.InvocationTarget, ADTOSharpCrossCuttingConcerns.Validation))
            {
                invocation.Proceed();
                return;
            }

            using (var validator = _iocResolver.ResolveAsDisposable<MethodInvocationValidator>())
            {
                validator.Object.Initialize(invocation.MethodInvocationTarget, invocation.Arguments);
                validator.Object.Validate();
            }
            
            invocation.Proceed();
        }


        protected override async Task InternalInterceptAsynchronous(IInvocation invocation)
        {
            var proceedInfo = invocation.CaptureProceedInfo();

            if (ADTOSharpCrossCuttingConcerns.IsApplied(invocation.InvocationTarget, ADTOSharpCrossCuttingConcerns.Validation))
            {
                proceedInfo.Invoke();
                await ((Task)invocation.ReturnValue);
                return;
            }

            using (var validator = _iocResolver.ResolveAsDisposable<MethodInvocationValidator>())
            {
                validator.Object.Initialize(invocation.MethodInvocationTarget, invocation.Arguments);
                validator.Object.Validate();
            }

            proceedInfo.Invoke();
            await ((Task)invocation.ReturnValue);
        }

        protected override async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation)
        {
            var proceedInfo = invocation.CaptureProceedInfo();

            if (ADTOSharpCrossCuttingConcerns.IsApplied(invocation.InvocationTarget, ADTOSharpCrossCuttingConcerns.Validation))
            {
                proceedInfo.Invoke();
                return await ((Task<TResult>)invocation.ReturnValue);
            }

            using (var validator = _iocResolver.ResolveAsDisposable<MethodInvocationValidator>())
            {
                validator.Object.Initialize(invocation.MethodInvocationTarget, invocation.Arguments);
                validator.Object.Validate();
            }

            proceedInfo.Invoke();
            return await ((Task<TResult>)invocation.ReturnValue);
        }
    }
}
