/***********************************************************/
/* NJAGE Engine - .NET Core Internals                      */
/*                                                         */
/* Copyright 2020 Marcel Bulla. All rights reserved.       */
/* Licensed under the MIT License. See LICENSE in the      */
/* project root for license information.                   */
/***********************************************************/

using System;

namespace De.Markellus.Njage.NetInternals
{
    public static class njRuntimeVerifier
    {
        /// <summary>
        /// Verifies the runtime.
        /// </summary>
        public static void Verify()
        {
            if(!Is64BitConfiguration())
            {
                Environment.Exit(int.MinValue);
            }
        }

        /// <summary>
        /// Returns true if the runtime is 64 Bit.
        /// </summary>
        public static bool Is64BitConfiguration()
        {
            return IntPtr.Size == 8;
        }
    }
}
