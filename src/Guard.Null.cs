﻿namespace Dawn
{
    using System;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;
    using JetBrains.Annotations;

    /// <content>Nullability preconditions.</content>
    public static partial class Guard
    {
        /// <summary>Requires the argument to be <c>null</c>.</summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="argument">The argument.</param>
        /// <param name="message">
        ///     The factory to initialize the message of the exception that will be thrown if the
        ///     precondition is not satisfied.
        /// </param>
        /// <returns><paramref name="argument" />.</returns>
        /// <exception cref="ArgumentException"><paramref name="argument" /> is not <c>null</c>.</exception>
        [AssertionMethod]
        [DebuggerStepThrough]
        [GuardFunction("Null", "gn")]
        public static ref readonly ArgumentInfo<T> Null<T>(
            in this ArgumentInfo<T> argument, Func<T, string> message = null)
            where T : class
        {
            if (argument.HasValue())
            {
                var m = message?.Invoke(argument.Value) ?? Messages.Null(argument);
                throw argument.Exception(new ArgumentException(m, argument.Name));
            }

            return ref argument;
        }

        /// <summary>Requires the nullable argument to be <c>null</c>.</summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="argument">The argument.</param>
        /// <param name="message">
        ///     The factory to initialize the message of the exception that will be thrown if the
        ///     precondition is not satisfied.
        /// </param>
        /// <returns><paramref name="argument" />.</returns>
        /// <exception cref="ArgumentException"><paramref name="argument" /> is not <c>null</c>.</exception>
        /// <remarks>
        ///     The argument value that is passed to <paramref name="message" /> cannot be
        ///     <c>null</c>, but it is defined as nullable anyway. This is because passing a lambda
        ///     would cause the calls to be ambiguous between this method and its overload when the
        ///     message delegate accepts a non-nullable argument.
        /// </remarks>
        [AssertionMethod]
        [DebuggerStepThrough]
        [GuardFunction("Null", "gn")]
        public static ref readonly ArgumentInfo<T?> Null<T>(
            in this ArgumentInfo<T?> argument, Func<T?, string> message = null)
            where T : struct
        {
            if (argument.HasValue())
            {
                Debug.Assert(argument.Value.HasValue, "argument.HasValue");
                var m = message?.Invoke(argument.Value.Value) ?? Messages.Null(argument);
                throw argument.Exception(new ArgumentException(m, argument.Name));
            }

            return ref argument;
        }

        /// <summary>Requires the argument not to be <c>null</c>.</summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="argument">The argument.</param>
        /// <param name="message">
        ///     The factory to initialize the message of the exception that will be thrown if the
        ///     precondition is not satisfied.
        /// </param>
        /// <returns><paramref name="argument" />.</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="argument" /> value is <c>null</c> and the argument is not modified
        ///     since it is initialized.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="argument" /> value is <c>null</c> and the argument is modified after
        ///     its initialization.
        /// </exception>
        [AssertionMethod]
        [DebuggerStepThrough]
        [GuardFunction("Null", "gnn")]
        public static ref readonly ArgumentInfo<T> NotNull<T>(
            in this ArgumentInfo<T> argument, string message = null)
            where T : class
        {
            if (!argument.HasValue())
            {
                var m = message ?? Messages.NotNull(argument);
                throw argument.Exception(!argument.Modified
                    ? new ArgumentNullException(argument.Name, m)
                    : new ArgumentException(m, argument.Name));
            }

            return ref argument;
        }

        /// <summary>Requires the nullable argument not to be <c>null</c>.</summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="argument">The argument.</param>
        /// <param name="message">
        ///     The factory to initialize the message of the exception that will be thrown if the
        ///     precondition is not satisfied.
        /// </param>
        /// <returns>A new <see cref="ArgumentInfo{T}" />.</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="argument" /> value is <c>null</c> and the argument is not modified
        ///     since it is initialized.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="argument" /> value is <c>null</c> and the argument is modified after
        ///     its initialization.
        /// </exception>
        [AssertionMethod]
        [DebuggerStepThrough]
        [GuardFunction("Null", "gnn")]
        public static ArgumentInfo<T> NotNull<T>(
            in this ArgumentInfo<T?> argument, string message = null)
            where T : struct
        {
            if (!argument.HasValue())
            {
                var m = message ?? Messages.NotNull(argument);
                throw argument.Exception(!argument.Modified
                    ? new ArgumentNullException(argument.Name, m)
                    : new ArgumentException(m, argument.Name));
            }

            return new ArgumentInfo<T>(
                argument.Scope,argument.Value.Value, argument.Name, argument.Modified, argument.Secure);
        }

        /// <summary>
        ///     Initializes a new <see cref="ArgumentInfo{T}" /> if the argument value is not
        ///     <c>null</c>. A return value indicates whether the new argument is created.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="result">
        ///     The new argument, if <paramref name="argument" /> is not <c>null</c>; otherwise, the
        ///     uninitialized argument.
        /// </param>
        /// <returns>
        ///     <c>true</c>, if the <paramref name="argument" /> is not <c>null</c>; otherwise, <c>false</c>.
        /// </returns>
        [AssertionMethod]
        [DebuggerStepThrough]
        [Obsolete("Use the HasValue method to check against null.")]
        public static bool NotNull<T>(
            in this ArgumentInfo<T?> argument, out ArgumentInfo<T> result)
            where T : struct
        {
            if (argument.HasValue())
            {
                result = new ArgumentInfo<T>(
                    argument.Scope,argument.Value.Value, argument.Name, argument.Modified, argument.Secure);

                return true;
            }

            result = default;
            return false;
        }

        /// <summary>Requires at least one of the specified arguments not to be <c>null</c>.</summary>
        /// <typeparam name="T1">The type of the first argument.</typeparam>
        /// <typeparam name="T2">The type of the second argument.</typeparam>
        /// <param name="argument1">The first argument.</param>
        /// <param name="argument2">The second argument.</param>
        /// <param name="message">
        ///     The factory to initialize the message of the exception that will be thrown if the
        ///     precondition is not satisfied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     None of the specified arguments have value.
        /// </exception>
        [AssertionMethod]
        [DebuggerStepThrough]
        [GuardFunction("Null")]
        public static void NotAllNull<T1, T2>(
            in ArgumentInfo<T1> argument1, in ArgumentInfo<T2> argument2, string message = null)
        {
            if (!argument1.HasValue() && !argument2.HasValue())
            {
                var m = message ?? Messages.NotAllNull(argument1.Name, argument2.Name);
                throw new ArgumentNullException($"{argument1.Name}, {argument2.Name}", m);
            }
        }

        /// <summary>Requires at least one of the specified arguments not to be <c>null</c>.</summary>
        /// <typeparam name="T1">The type of the first argument.</typeparam>
        /// <typeparam name="T2">The type of the second argument.</typeparam>
        /// <typeparam name="T3">The type of the third argument.</typeparam>
        /// <param name="argument1">The first argument.</param>
        /// <param name="argument2">The second argument.</param>
        /// <param name="argument3">The third argument.</param>
        /// <param name="message">
        ///     The factory to initialize the message of the exception that will be thrown if the
        ///     precondition is not satisfied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     None of the specified arguments have value.
        /// </exception>
        [AssertionMethod]
        [DebuggerStepThrough]
        [GuardFunction("Null")]
        public static void NotAllNull<T1, T2, T3>(
            in ArgumentInfo<T1> argument1,
            in ArgumentInfo<T2> argument2,
            in ArgumentInfo<T3> argument3,
            string message = null)
        {
            if (!argument1.HasValue() && !argument2.HasValue() && !argument3.HasValue())
            {
                var m = message ?? Messages.NotAllNull(argument1.Name, argument2.Name, argument3.Name);
                throw new ArgumentNullException($"{argument1.Name}, {argument2.Name}, {argument3.Name}", m);
            }
        }

        /// <summary>Provides a <c>null</c> checking helper.</summary>
        /// <typeparam name="T">The type of the instance to check against <c>null</c>.</typeparam>
        private static class NullChecker<T>
        {
            /// <summary>
            ///     A function that determines whether a specified instance of type
            ///     <typeparamref name="T" /> is not <c>null</c>.
            /// </summary>
            private static readonly IsNotNull HasValueImpl = InitHasValue();

            /// <summary>A delegate that checks whether an object is not <c>null</c>.</summary>
            /// <param name="value">The value to check against <c>null</c>.</param>
            /// <returns>
            ///     <c>true</c>, if <paramref name="value" /> is not <c>null</c>; otherwise, <c>false</c>.
            /// </returns>
            private delegate bool IsNotNull(in T value);

            /// <summary>
            ///     Determines whether a specified instance of type <typeparamref name="T" /> is not <c>null</c>.
            /// </summary>
            /// <param name="value">The value to check against <c>null</c>.</param>
            /// <returns>
            ///     <c>true</c>, if <paramref name="value" /> is not <c>null</c>; otherwise, <c>false</c>.
            /// </returns>
            [ContractAnnotation("value:notnull => true; value:null => false")]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool HasValue(in T value) => HasValueImpl(value);

            /// <summary>Initializes <see cref="HasValue" />.</summary>
            /// <returns>
            ///     A function that determines whether a specified instance of type
            ///     <typeparamref name="T" /> is not <c>null</c>.
            /// </returns>
            private static IsNotNull InitHasValue()
            {
                var type = typeof(T);
                if (!type.IsValueType())
                    return (in T v) => v != null;

                if (type.IsGenericType(typeof(Nullable<>)))
                {
                    var value = Expression.Parameter(type.MakeByRefType(), "value");
                    var get = type.GetPropertyGetter("HasValue");
                    var call = Expression.Call(value, get);
                    var lambda = Expression.Lambda<IsNotNull>(call, value);
                    return lambda.Compile();
                }

                return (in T v) => true;
            }
        }
    }
}
