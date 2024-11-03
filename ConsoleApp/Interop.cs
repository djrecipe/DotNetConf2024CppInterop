using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal abstract class BasicInterop
    {
        [DllImport("InteropExample.dll")]
        public static extern int Add(int left, int right);

        [DllImport("InteropExample.dll")]
        public static extern int MyMangledName();

        [DllImport("InteropExample.dll",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern int MYcdecl();

        [DllImport("InteropExample.dll",
            CallingConvention = CallingConvention.StdCall)]
        public static extern int MYstdcall();
    }

    internal abstract class Interop
    {
        internal abstract class Delegates
        {
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate int Add(int left, int right);
        }

        private static IntPtr library;
        public static void Initialize(IntPtr lib)
        {
            library = lib;
        }

        public static int Add(int left, int right)
        {
            return 0;
        }
    }
}