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

        [DllImport("InteropExample.dll",
            CallingConvention = CallingConvention.Cdecl)]
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
            internal delegate int ConcatStrings(
                [MarshalAs(UnmanagedType.LPStr)] string left,
                [MarshalAs(UnmanagedType.LPStr)] string right,
                out IntPtr output);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate int DeleteArray(IntPtr ptr);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate int MyMangledName();
        }

        private static IntPtr library;
        public static void Initialize(string path)
        {
            library = NativeLibrary.Load(path);
        }

        public static int Add(int left, int right)
        {
            IntPtr func = NativeLibrary.GetExport(library, "Add");
            Add method = (Add)Marshal.GetDelegateForFunctionPointer(
                func, typeof(Add));
            return method(left, right);
        }

        private static int DeleteArray(IntPtr ptr)
        {
            IntPtr func = NativeLibrary.GetExport(library, "DeleteArray");

            DeleteArray method = (DeleteArray)Marshal.GetDelegateForFunctionPointer(
                func,
                typeof(DeleteArray));

            return method(ptr);
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
            DeleteArray(ptr); // don't forget to release unmanaged memory

            return text;
        }

        public static int MyMangledName()
        {
            IntPtr func = NativeLibrary.GetExport(library, "MyMangledName");
            if (func == IntPtr.Zero)
                throw new Exception("Failed to find function with name 'MyMangledName'");

            MyMangledName method = (MyMangledName)Marshal.GetDelegateForFunctionPointer(
                func,
                typeof(MyMangledName));

            int result = method();
            return result;
        }
    }
}