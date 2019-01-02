using System;
using System.Diagnostics;
using System.Linq.Expressions;
using JetBrains.Annotations;
// ReSharper disable ArrangeThisQualifier

namespace Dawn.Scope
{
    /// <summary>
    ///  Guard scope implementation.
    /// </summary>
    /// <remarks>
    ///  Scope is currently used for exception interceptors only.
    /// </remarks>
    internal class GuardScope : GuardDisposable, IGuardScope
    {
        #region Root scope
        /// <summary>
        /// Instance of the <see cref="GuardScope"/> created when the singleton is first touched - root scope
        /// </summary>
        /// <remarks>
        /// Not using the auto-property to have better control, when the instance is created
        /// </remarks>
        // ReSharper disable once InconsistentNaming
        private static readonly GuardScope RootInternal = new GuardScope();

        /// <summary>
        /// Gets the root scope.
        /// </summary>
        // ReSharper disable once ConvertToAutoProperty
        public static IGuardScope Root => RootInternal;

        /// <summary>
        /// Static constructor
        /// </summary>
        /// <remarks>Explicit static constructor to tell C# compiler not to mark type as beforefieldinit </remarks>
        static GuardScope() { }
        #endregion

        /// <summary>
        /// Gets the disposer associated with this container. Instances can be associated
        /// with it manually if required.
        /// </summary>
        private GuardDisposer GuardDisposer { get; }

        /// <summary>
        /// Gets the parent scope.
        /// </summary>
        public IGuardScope Parent { get; }

        /// <summary>
        ///  Exception interceptor used just before the exception is thrown
        /// </summary>
        private readonly Action<Exception> exceptionInterceptor;

        /// <summary>
        ///  Gets an exception interceptor used just before the exception is thrown
        /// </summary>
        public Action<Exception> ExceptionInterceptor
        {
            get
            {
                this.AssertNotDisposed();
                return this.exceptionInterceptor;
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="GuardScope"/> and initialize its <see cref="GuardDisposer"/>.
        /// </summary>
        private GuardScope()
        {
            this.GuardDisposer = new GuardDisposer();
        }

        /// <summary>
        ///  Creates an instance of <see cref="GuardScope"/>.
        /// </summary>
        /// <param name="parent">Parent scope</param>
        /// <param name="exceptionInterceptor">Optional Exception interceptor managed by the scope.</param>
        protected GuardScope(IGuardScope parent, Action<Exception> exceptionInterceptor=null) : this()
        {
            this.Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            this.exceptionInterceptor = exceptionInterceptor;
        }

        /// <summary>
        ///  Creates a child scope of the current scope.
        /// </summary>
        /// <param name="scopeExceptionInterceptor">Optional Exception interceptor managed by the child scope.</param>
        /// <returns>Child scope owned by the current scope.</returns>
        /// <exception cref="ObjectDisposedException">The current scope has been disposed.</exception>
        public IGuardScope BeginScope(Action<Exception> scopeExceptionInterceptor=null)
        {
            this.AssertNotDisposed();

            var scope = new GuardScope(this, scopeExceptionInterceptor);
            this.GuardDisposer.Add(scope);
            return scope;
        }


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
        /// <exception cref="ObjectDisposedException">Current scope is disposed</exception>
        [DebuggerStepThrough]
        public Guard.ArgumentInfo<T> Argument<T>(
            T value, [InvokerParameterName] string name = null, bool secure = false)
            => new Guard.ArgumentInfo<T>(this, value, name, secure: secure);

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
        /// <exception cref="ArgumentNullException"><paramref name="e" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="e" /> is not a <see cref="MemberExpression" />.</exception>
        /// <exception cref="ObjectDisposedException">Current scope is disposed</exception>
        [DebuggerStepThrough]
        public Guard.ArgumentInfo<T> Argument<T>(Expression<Func<T>> e, bool secure = false)
        {
            this.AssertNotDisposed();

            if (e == null)
                throw new ArgumentNullException(nameof(e));

            return e.Body is MemberExpression m
                ? this.Argument(e.Compile()(), m.Member.Name, secure)
                : throw new ArgumentException("A member expression is expected.", nameof(e));
        }

        /// <summary>
        /// Internal implementation of dispose - free the managed and native resources.
        /// </summary>
        /// <remarks>
        /// When disposing the managed objects (<paramref name="disposing"/> is true),
        /// all disposables kept in <see cref="GuardDisposer"/> stack are pop and disposed.
        /// </remarks>
        /// <param name="disposing">True to dispose both managed and native resources, false to dispose the native resources only.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.GuardDisposer.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        ///  Throws an <see cref="ObjectDisposedException"/> when the current object is disposed
        /// </summary>
        /// <exception cref="ObjectDisposedException">Current object is disposed</exception>
        private void AssertNotDisposed()
        {
            if (this.IsDisposed)
                throw new ObjectDisposedException("Guard scope is disposed");
        }
    }


}
