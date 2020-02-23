using System;
using System.Reflection;
using De.Markellus.Njage.NetInternals;

namespace Markellus.Njage.NetInternals
{
    public static class njGenericRuntimeCaller
    {
        static njGenericRuntimeCaller()
        {
            njRuntimeVerifier.Verify();
        }
        
        /// <summary>
        /// Dynamically compiles and calls a generic function if only a type object is available at runtime.
        /// </summary>
        /// <param name="tClass">The type of the class that contains the generic method.</param>
        /// <param name="instance">Object instance on which the method is applied, or null if it is a static function. The object must be of type <see cref="typeClass"/>.</param>
        /// <param name="strMethod">The name of the generic method to be recompiled.</param>
        /// <param name="bindingFlags">Method Flags</param>
        /// <param name="typeTarget">The type of the generic function, usually enclosed in triangular brackets (<>).</param>
        /// <param name="arguments">Arguments of the generic function</param>
        /// <returns>The return value of the function, if available.</returns>
        public static object Invoke(Type tClass, object instance, string strMethod, BindingFlags bindingFlags,
            Type typeTarget, params object[] arguments)
        {
            MethodInfo method = tClass.GetMethod(strMethod, bindingFlags);
            method = method?.MakeGenericMethod(typeTarget);
            return method?.Invoke(instance, arguments);
        }

        /// <summary>
        /// Dynamically compiles a generic type of this base type.
        /// </summary>
        /// <param name="tGenericArg">The generic argument</param>
        /// <param name="arrConstructorArgs">Arguments of the constructor</param>
        /// <typeparam name="T">The type of the class</typeparam>
        /// <returns>A new instance of <see cref="tTarget"/> with the specified generic argument.</returns>
        public static T Invoke<T>(Type tGenericArg, params object[] arrConstructorArgs)
        {
            return (T) Invoke(typeof(T), tGenericArg, arrConstructorArgs);
        }

        /// <summary>
        /// Dynamically compiles a generic type if only a type object is available at runtime.
        /// </summary>
        /// <param name="tTarget">The type of the class</param>
        /// <param name="tGenericArg">The generic argument</param>
        /// <param name="arrConstructorArgs">Arguments of the constructor</param>
        /// <returns>A new instance of <see cref="tTarget"/> with the specified generic argument.</returns>
        public static object Invoke(Type tTarget, Type tGenericArg, params object[] arrConstructorArgs)
        {
            Type typeGeneric = tTarget.MakeGenericType(tGenericArg);

            return Activator.CreateInstance(typeGeneric,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, arrConstructorArgs, null);
        }
    }
}