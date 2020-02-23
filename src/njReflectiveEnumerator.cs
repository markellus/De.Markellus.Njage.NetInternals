/***********************************************************/
/* NJAGE Engine - .NET Core Internals                      */
/*                                                         */
/* Copyright 2020 Marcel Bulla. All rights reserved.       */
/* Licensed under the MIT License. See LICENSE in the      */
/* project root for license information.                   */
/***********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace De.Markellus.Njage.NetInternals
{
    public static class njReflectiveEnumerator
    {
        static njReflectiveEnumerator()
        {
            njRuntimeVerifier.Verify();
        }

        /// <summary>
        /// Returns an instance of each class of all loaded assemblies with a specified base type.
        /// </summary>
        /// <param name="constructorArgs">Constructor arguments for all new instances</param>
        /// <typeparam name="T">The base type of all classes</typeparam>
        public static IEnumerable<T> GetInstancesOfType<T>(params object[] constructorArgs) where T : class
        {
            Assembly[] arrAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in arrAssemblies)
            {
                foreach (T t in GetInstancesOfType<T>(assembly, constructorArgs))
                {
                    yield return t;
                }
            }
        }

        /// <summary>
        /// Returns an instance of each class with a specified base type.
        /// </summary>
        /// <param name="assembly">The assembly which contains the target classes</param>
        /// <param name="constructorArgs">Constructor arguments for all new instances</param>
        /// <typeparam name="T">The base type of all classes</typeparam>
        public static IEnumerable<T> GetInstancesOfType<T>(Assembly assembly, params object[] constructorArgs) where T : class
        {
            try
            {
                //.NET Core 3.x Bug: Unit tests can load assemblies that are invalid.
                //Such an assembly will throw an exception when meta information of any type is accessed. 
                var info = assembly.DefinedTypes;
            }
            catch
            {
                return new List<T>(0);
            }
            
            return assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(T)))
                .Select(t => (T)Activator.CreateInstance(t,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, constructorArgs, null));
        }

        /// <summary>
        /// Tries to find a type object that matches the given name.
        /// </summary>
        /// <param name="strType">The type as a string</param>
        /// <returns>A type object or null if there is no matching type.</returns>
        public static Type GetTypeFromString(string strType)
        {
            Assembly[] arrAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in arrAssemblies)
            {
                try
                {
                    //.NET Core 3.x Bug: Unit tests can load assemblies that are invalid.
                    //Such an assembly will throw an exception when meta information of any type is accessed. 
                    var info = assembly.DefinedTypes;
                }
                catch
                {
                    continue;
                }

                foreach (Type type in assembly.GetTypes())
                {
                    if (type.FullName == strType)
                    {
                        return type;
                    }
                }
            }

            return null;
        }

        /*// <summary>
            /// Returns an instance of each class of all loaded assemblies which are derived from a specified interface.
            /// </summary>
            /// <param name="assembly"></param>
            /// <param name="constructorArgs">Constructor arguments for all new instances</param>
            /// <typeparam name="T">The interface of all classes</typeparam>
        public static T GetInstanceOfInterface<T>(Assembly assembly, params object[] constructorArgs)
        {
            Type type = assembly.GetTypes()
                .FirstOrDefault(t => t.IsClass && !t.IsAbstract && typeof(T).IsAssignableFrom(t));

            if (type == null)
            {
                return default;
            }

            return (T) Activator.CreateInstance(type,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                null, constructorArgs, null);
        }*/
    }
}