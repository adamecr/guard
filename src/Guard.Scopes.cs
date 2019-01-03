﻿#if !NETSTANDARD1_0

namespace Dawn
{
    using System;
    using System.Threading;

    /// <content>Provides scoping support.</content>
    public static partial class Guard
    {
        /// <summary>Starts a guarding scope using the specified exception interceptor.</summary>
        /// <param name="exceptionInterceptor">
        ///     A delegate to intercept the exceptions caused by failed validations.
        /// </param>
        /// <returns>An object that when disposed, will end the guarding scope.</returns>
        public static IDisposable BeginScope(Action<Exception> exceptionInterceptor)
            => exceptionInterceptor is null ? Disposable.Empty : new Scope(exceptionInterceptor);

        /// <summary>Represents a guarding scope.</summary>
        private sealed class Scope : IDisposable
        {
            /// <summary>
            ///     The scope data that is local to the calling asynchronous control flow.
            /// </summary>
            private static readonly AsyncLocal<Scope> Local = new AsyncLocal<Scope>();

            /// <summary>
            ///     Contains zero if the instance is not disposed; and one if it is disposed.
            /// </summary>
            private int disposed;

            /// <summary>Initializes a new instance of the <see cref="Scope" /> class.</summary>
            /// <param name="exceptionInterceptor">
            ///     A delegate to intercept the exceptions caused by failed validations.
            /// </param>
            public Scope(Action<Exception> exceptionInterceptor)
            {
                this.Parent = Current;
                Current = this;

                this.ExceptionInterceptor = exceptionInterceptor;
            }

            /// <summary>Gets the previous scope to restore when the current one is disposed.</summary>
            public Scope Parent { get; }

            /// <summary>Gets the current guarding scope.</summary>
            public static Scope Current
            {
                get => Local.Value;
                private set => Local.Value = value;
            }

            /// <summary>Gets a delegate to intercept the exceptions caused by failed validations.</summary>
            public Action<Exception> ExceptionInterceptor { get; }

            /// <summary>Ends the guarding scope.</summary>
            public void Dispose()
            {
                if (Interlocked.CompareExchange(ref this.disposed, 1, 0) == 0)
                    Current = this.Parent;
            }
        }

        /// <summary>Provides helpers to create disposables.</summary>
        private static class Disposable
        {
            /// <summary>Gets an instance that does nothing when disposed.</summary>
            public static IDisposable Empty { get; } = new EmptyDisposable();

            /// <summary>
            ///     An <see cref="IDisposable" /> implementation that does nothing when disposed.
            /// </summary>
            private sealed class EmptyDisposable : IDisposable
            {
                /// <summary>Does nothing.</summary>
                public void Dispose()
                {
                }
            }
        }
    }
}

#endif