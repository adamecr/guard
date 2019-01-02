using System;
using System.Threading;
// ReSharper disable ArrangeThisQualifier

namespace Dawn.Scope
{
    /// <summary>
    /// Helper class for implementation of <see cref="IDisposable" /> types.
    /// </summary>
    public class GuardDisposable : IDisposable
    {
        private const int DisposedFlag = 1;
        private int isDisposed;

        /// <summary>
        /// Disposes the object
        /// </summary>
        public void Dispose()
        {
            if(Interlocked.Exchange(ref this.isDisposed, DisposedFlag) == DisposedFlag) return; //already disposed
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Internal implementation of dispose - free the managed and native resources.
        /// </summary>
        /// <param name="disposing">True to dispose both managed and native resources, false to dispose the native resources only.</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Flag whether the object has been disposed.
        /// </summary>
        protected bool IsDisposed
        {
            get
            {
                Interlocked.MemoryBarrier();
                return this.isDisposed == DisposedFlag;
            }
        }
    }
}
