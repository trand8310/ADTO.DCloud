using System.Threading.Tasks;
using ADTOSharp.Dependency;
using Castle.DynamicProxy;

namespace ADTOSharp.Authorization
{
    /// <summary>
    /// This class is used to intercept methods to make authorization if the method defined <see cref="ADTOSharpAuthorizeAttribute"/>.
    /// </summary>
    public class AuthorizationInterceptor : ADTOSharpInterceptorBase, ITransientDependency
    {
        private readonly IAuthorizationHelper _authorizationHelper;

        public AuthorizationInterceptor(IAuthorizationHelper authorizationHelper)
        {
            _authorizationHelper = authorizationHelper;
        }

        public override void InterceptSynchronous(IInvocation invocation)
        {
            _authorizationHelper.Authorize(invocation.MethodInvocationTarget, invocation.TargetType);
            invocation.Proceed();
        }

        protected override async Task InternalInterceptAsynchronous(IInvocation invocation)
        {
            var proceedInfo = invocation.CaptureProceedInfo();
            
            await _authorizationHelper.AuthorizeAsync(invocation.MethodInvocationTarget, invocation.TargetType);

            proceedInfo.Invoke();
            var task = (Task)invocation.ReturnValue;
            await task;
        }
        
        protected override async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation)
        {
            var proceedInfo = invocation.CaptureProceedInfo();

            await _authorizationHelper.AuthorizeAsync(invocation.MethodInvocationTarget, invocation.TargetType);

            proceedInfo.Invoke();
            var taskResult = (Task<TResult>)invocation.ReturnValue;
            return await taskResult;
        }
    }
}
