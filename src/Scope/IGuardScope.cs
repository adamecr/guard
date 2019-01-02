using System;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace Dawn.Scope
{
    /// <summary>
    ///  Guard scope interface.
    /// </summary>
    /// <remarks>
    ///  Scope is currently used for exception interceptors only.
    /// </remarks>
    public interface IGuardScope : IDisposable
    {
        /// <summary>
        ///  Gets an exception interceptor used just before the exception is thrown
        /// </summary>
        Action<Exception> ExceptionInterceptor { get; }

        /// <summary>
        ///  Creates a child scope of the current scope.
        /// </summary>
        /// <param name="exceptionInterceptor">Exception interceptor managed by the child scope.</param>
        /// <returns>Child scope owned by the current scope.</returns>
        IGuardScope BeginScope(Action<Exception> exceptionInterceptor);

        /// <summary>
        ///     Returns an object that can be used to assert preconditions for the method argument
        ///     with the specified name and value in the current scope.
        /// </summary>
        /// <typeparam name="T">The type of the method argument.</typeparam>
        /// <param name="value">The value of the method argument.</param>
        /// <param name="name">
        ///     <para>
        ///         The name of the method argument. Use the <c>nameof</c> operator ( <c>Nameof</c>
        ///         in Visual Basic) where possible.
        ///     </para>
        ///     <para>
        ///         It is highly recommended you don't left this value <c>null</c> so the arguments
        ///         violating the preconditions can be easily identified.
        ///     </para>
        /// </param>
        /// <param name="secure">
        ///     Pass <c>true</c> for the validation parameters to be excluded from the exception
        ///     messages of failed validations.
        /// </param>
        /// <returns>An object used for asserting preconditions.</returns>
        Guard.ArgumentInfo<T> Argument<T>(T value, [InvokerParameterName] string name = null, bool secure = false);

        /// <summary>
        ///     Returns an object that can be used to assert preconditions for the specified method argument in current scope.
        /// </summary>
        /// <typeparam name="T">The type of the method argument.</typeparam>
        /// <param name="e">An expression that specifies a method argument.</param>
        /// <param name="secure">
        ///     Pass <c>true</c> for the validation parameters to be excluded from the exception
        ///     messages of failed validations.
        /// </param>
        /// <returns>An object used for asserting preconditions.</returns>
        Guard.ArgumentInfo<T> Argument<T>(Expression<Func<T>> e, bool secure = false);
    }
}
