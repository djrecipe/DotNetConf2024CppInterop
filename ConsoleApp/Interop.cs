using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApp.Interop.Delegates;

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

        [DllImport("InteropExample.dll")]
        public static extern int ConcatStrings(
            [MarshalAs(UnmanagedType.LPStr)] string left,
            [MarshalAs(UnmanagedType.LPStr)] string right,
            out IntPtr output);
    }

    // advantages: better path resolution, potentially faster runtime
    internal abstract class Interop
    {
        internal abstract class Delegates
        {
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate int Add(int left, int right);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate int ConcatStrings(string left, string right, out IntPtr output);
        }

        private static IntPtr library;
        public static void Initialize(string path)
        {
            library = NativeLibrary.Load(path);
        }

        public static int Add(int left, int right)
        {
            IntPtr pAddressOfFunctionToCall = NativeLibrary.GetExport(library, "Add");

            Add method = (Add)Marshal.GetDelegateForFunctionPointer(
                pAddressOfFunctionToCall,
                typeof(Add));

            return method(left, right);
        }

        public static string ConcatStrings(string left, string right)
        {
            IntPtr func = NativeLibrary.GetExport(library, "ConcatStrings");

            ConcatStrings method = (ConcatStrings)Marshal.GetDelegateForFunctionPointer(
                func,
                typeof(ConcatStrings));

            IntPtr ptr = IntPtr.Zero;
            int count = method(left, right, out ptr);

            var text = Marshal.PtrToStringUTF8(ptr);
            Marshal.FreeHGlobal(ptr); // don't forget to release unmanaged memor

            return text;
        }
    }
}