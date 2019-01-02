using System;
using System.Diagnostics;

namespace Dawn
{
    //JUST THE PoC - the Soft<T> check is quite resource heavy
    public partial class Guard
    {
        /// <summary>
        ///     Checks the argument using the <paramref name="checkFunction"/>.
        ///     When the check fails, the <paramref name="correctionFunction"/> is applied to argument
        ///     with optional re-check. When the re-check fails, the exception is thrown.
        /// <para>
        ///    JUST THE PoC - the <see cref="Soft{T}"/> check is quite resource heavy
        /// </para>
        /// </summary>
        /// <typeparam name="T">
        ///     The type that the argument's value should be an instance of.
        /// </typeparam>
        /// <param name="argument">The object argument.</param>
        /// <param name="checkFunction">
        /// Argument check function (may be a chain of checks).
        /// The check function must keep the argument type.
        /// </param>
        /// <param name="correctionFunction">Argument correction function.</param>
        /// <param name="message">Error message used when the correction function fails </param>
        /// <param name="recheck">Flag whether the re-check the corrected argument</param>
        /// <returns>A new <see cref="ArgumentInfo{T}" />.</returns>
        /// <exception cref="Exception">
        ///     <paramref name="argument" /> value is not an instance of type <typeparamref name="T" />.
        /// </exception>
        [DebuggerStepThrough]
        public static ArgumentInfo<T> Soft<T>(
            in this ArgumentInfo<T> argument,
            Func<ArgumentInfo<T>, ArgumentInfo<T>> checkFunction,
            Func<ArgumentInfo<T>, T> correctionFunction,
            Func<T, string> message = null,
            bool recheck = false)

        {
            try
            {
                //Check and if OK return the result
                return checkFunction(argument);
            }
            catch (Exception)
            {
                ArgumentInfo<T> retVal;
                try
                {
                    //Try to correct
                    retVal = argument.Modify(correctionFunction(argument));
                }
                catch (Exception ex)
                {
                    //Can't correct/modify
                    var m = message?.Invoke(argument.Value) ?? Messages.Require(argument);
                    throw argument.Exception(new ArgumentException(m, argument.Name, ex));
                }
                //Recheck if needed (without additional correction)
                return !recheck ? retVal : checkFunction(retVal);
            }
        }
    }
}
