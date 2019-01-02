using System;
using Xunit;

namespace Dawn.Tests
{
    public class InterceptorBasicTests : BaseTests
    {
        [Theory(DisplayName = "Intercept test")]
        [InlineData("A")]
        public void Intercept(string argument)
        {
            var intercepted = false;
            Exception interceptedException = null;

            using (var scope = Guard.BeginScope(ex =>
              {
                  intercepted = true;
                  interceptedException = ex;
              }))
            {
                var emptyArg = scope.Argument(() => argument);

                var exception = Assert.Throws<ArgumentException>(() => emptyArg.Empty());

                Assert.True(intercepted);
                Assert.Equal(exception, interceptedException);
            }
        }

        [Theory(DisplayName = "No exception")]
        [InlineData("")]
        public void NoException(string argument)
        {
            var intercepted = false;
            Exception interceptedException = null;

            Guard.Argument(
                () => argument,
                exceptionInterceptor: (ex) =>
                {
                    intercepted = true;
                    interceptedException = ex;
                });

            Assert.False(intercepted);
            Assert.Null(interceptedException);
        }

        [Theory(DisplayName = "Modified argument info test")]
        [InlineData("A")]
        public void Modified(string argument)
        {
            var intercepted = false;
            Exception interceptedException = null;

            using (var scope = Guard.BeginScope(ex =>
              {
                  intercepted = true;
                  interceptedException = ex;
              }))
            {
                var emptyArg = scope.Argument(() => argument);

                var exception = Assert.Throws<ArgumentException>(() =>
                    emptyArg
                        .NotEmpty()
                        .Modify(s => "")
                        .NotEmpty()
                );

                Assert.True(intercepted);
                Assert.Equal(exception, interceptedException);
            }
        }

        [Theory(DisplayName = "No interceptor test")]
        [InlineData("A")]
        public void NoInterceptor(string argument)
        {
            var emptyArg = Guard.Argument(() => argument);
            Assert.Throws<ArgumentException>(() => emptyArg.Empty());
        }

        [Theory(DisplayName = "No interceptor for scope test")]
        [InlineData("A")]
        public void NoInterceptorInScope(string argument)
        {
            using (var scope = Guard.BeginScope())
            {
                var emptyArg = scope.Argument(() => argument);
                Assert.Throws<ArgumentException>(() => emptyArg.Empty());
            }
        }

        [Theory(DisplayName = "Exception in interceptor test")]
        [InlineData("A")]
        public void ExceptionInInterceptor(string argument)
        {
            using (var scope = Guard.BeginScope(ex => throw new InvalidOperationException()))
            {
                var emptyArg = scope.Argument(() => argument);

                //When the interceptor fails, its exception is thrown instead of the Guard exception (it's not "hidden" by design)
                Assert.Throws<InvalidOperationException>(() => emptyArg.Empty());
            }

        }

        [Theory(DisplayName = "No exception intercepted in root")]
        [InlineData("A")]
        public void NoExceptionInterceptInRoot(string argument)
        {
            var intercepted = false;
            Exception interceptedException = null;

            using (Guard.BeginScope(ex =>
            {
                intercepted = true;
                interceptedException = ex;
            }))
            {
                var emptyArg = Guard.Argument(() => argument); //Guard.Argument=use root scope, so the scope created is to be ignored

                Assert.Throws<ArgumentException>(() => emptyArg.Empty());
                Assert.False(intercepted);
                Assert.Null(interceptedException);
            }
        }

        [Theory(DisplayName = "No exception intercepted in parent")]
        [InlineData("A")]
        public void NoExceptionInterceptInParent(string argument)
        {
            var intercepted = false;
            Exception interceptedException = null;
            var interceptedInner = false;
            Exception interceptedExceptionInner = null;

            using (var scope = Guard.BeginScope(ex =>
            {
                intercepted = true;
                interceptedException = ex;
            }))
            {
                using (var scopeInner = scope.BeginScope(ex => //inner scope
                {
                    interceptedInner = true;
                    interceptedExceptionInner = ex;
                }))
                {
                    var emptyArg = scopeInner.Argument(() => argument); //in inner scope

                    var exception = Assert.Throws<ArgumentException>(() => emptyArg.Empty());
                    Assert.False(intercepted); //not intercepted in outer scope
                    Assert.Null(interceptedException);
                    Assert.True(interceptedInner); //intercepted in inner scope
                    Assert.Equal(exception, interceptedExceptionInner);
                }
            }
        }

        [Theory(DisplayName = "No exception intercepted in inner")]
        [InlineData("A")]
        public void NoExceptionInterceptInInner(string argument)
        {
            var intercepted = false;
            Exception interceptedException = null;
            var interceptedInner = false;
            Exception interceptedExceptionInner = null;

            using (var scope = Guard.BeginScope(ex =>
            {
                intercepted = true;
                interceptedException = ex;
            }))
            {
                using (scope.BeginScope(ex => //inner scope
                {
                    interceptedInner = true;
                    interceptedExceptionInner = ex;
                }))
                {
                    var emptyArg = scope.Argument(() => argument); //in  outer scope

                    var exception = Assert.Throws<ArgumentException>(() => emptyArg.Empty());
                    Assert.False(interceptedInner); //not intercepted in inner scope
                    Assert.Null(interceptedExceptionInner);
                    Assert.True(intercepted); //intercepted in outer scope
                    Assert.Equal(exception, interceptedException);
                }
            }
        }

        [Theory(DisplayName = "No exception intercepted in sibling")]
        [InlineData("A")]
        public void ExceptionInterceptInSibling(string argument)
        {
            var intercepted = false;
            Exception interceptedException = null;
            var interceptedSibling = false;
            Exception interceptedExceptionSibling = null;

            using (Guard.BeginScope(ex =>
            {
                intercepted = true;
                interceptedException = ex;
            }))
            {
                using (var scopeSibling = Guard.BeginScope(ex =>
                {
                    interceptedSibling = true;
                    interceptedExceptionSibling = ex;
                }))
                {
                    var emptyArg = scopeSibling.Argument(() => argument);

                    var exception = Assert.Throws<ArgumentException>(() => emptyArg.Empty());
                    Assert.False(intercepted); //not intercepted in the first child scope
                    Assert.Null(interceptedException);
                    Assert.True(interceptedSibling); //intercepted in the second child scope
                    Assert.Equal(exception, interceptedExceptionSibling);
                }
            }
        }

        [Theory(DisplayName = "Disposed scope test")]
        [InlineData("A")]
        public void DisposedScope(string argument)
        {
            var intercepted = false;
            Exception interceptedException = null;
            Guard.ArgumentInfo<string> emptyArg;
            using (var scope = Guard.BeginScope(ex =>
            {
                intercepted = true;
                interceptedException = ex;
            }))
            {
                emptyArg = scope.Argument(() => argument);
            }

            Assert.Throws<ObjectDisposedException>(() => emptyArg.Empty());

            Assert.False(intercepted);
            Assert.Null(interceptedException);

        }
    }
}
