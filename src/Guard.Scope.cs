using System;
using Dawn.Scope;

namespace Dawn
{
    public partial class Guard
    {
        /// <summary>
        ///  Creates a child scope of the Guard root scope.
        /// </summary>
        /// <param name="exceptionInterceptor">Optional Exception interceptor managed by the child scope.</param>
        /// <returns>Child scope owned by the root scope.</returns>
        public static IGuardScope BeginScope(Action<Exception> exceptionInterceptor=null)
        {
            return GuardScope.Root.BeginScope(exceptionInterceptor);
        }
    }

}
