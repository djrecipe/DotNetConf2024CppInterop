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
        public static extern int MyMangledName(int left, int right);
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