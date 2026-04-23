using System;
using System.Reflection;

namespace ADTOSharp.Aspects
{
    //THIS NAMESPACE IS WORK-IN-PROGRESS

    internal abstract class AspectAttribute : Attribute
    {
        public Type InterceptorType { get; set; }

        protected AspectAttribute(Type interceptorType)
        {
            InterceptorType = interceptorType;
        }
    }

    internal interface IADTOSharpInterceptionContext
    {
        object Target { get; }

        MethodInfo Method { get; }

        object[] Arguments { get; }

        object ReturnValue { get; }

        bool Handled { get; set; }
    }

    internal interface IADTOSharpBeforeExecutionInterceptionContext : IADTOSharpInterceptionContext
    {

    }


    internal interface IADTOSharpAfterExecutionInterceptionContext : IADTOSharpInterceptionContext
    {
        Exception Exception { get; }
    }

    internal interface IADTOSharpInterceptor<TAspect>
    {
        TAspect Aspect { get; set; }

        void BeforeExecution(IADTOSharpBeforeExecutionInterceptionContext context);

        void AfterExecution(IADTOSharpAfterExecutionInterceptionContext context);
    }

    internal abstract class ADTOSharpInterceptorBase<TAspect> : IADTOSharpInterceptor<TAspect>
    {
        public TAspect Aspect { get; set; }

        public virtual void BeforeExecution(IADTOSharpBeforeExecutionInterceptionContext context)
        {
        }

        public virtual void AfterExecution(IADTOSharpAfterExecutionInterceptionContext context)
        {
        }
    }

    internal class Test_Aspects
    {
        internal class MyAspectAttribute : AspectAttribute
        {
            public int TestValue { get; set; }

            public MyAspectAttribute()
                : base(typeof(MyInterceptor))
            {
            }
        }

        internal class MyInterceptor : ADTOSharpInterceptorBase<MyAspectAttribute>
        {
            public override void BeforeExecution(IADTOSharpBeforeExecutionInterceptionContext context)
            {
                Aspect.TestValue++;
            }

            public override void AfterExecution(IADTOSharpAfterExecutionInterceptionContext context)
            {
                Aspect.TestValue++;
            }
        }

        public class MyService
        {
            [MyAspect(TestValue = 41)] //Usage!
            public void DoIt()
            {

            }
        }

        public class MyClient
        {
            private readonly MyService _service;

            public MyClient(MyService service)
            {
                _service = service;
            }

            public void Test()
            {
                _service.DoIt();
            }
        }
    }
}
