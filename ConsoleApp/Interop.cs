﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApp.Interop.Delegates;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            internal delegate int ConcatWideStrings(
                IntPtr left,
                IntPtr right,
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

        public static string ConcatWideStrings(string left, string right)
        {
            IntPtr func = NativeLibrary.GetExport(library, "ConcatWideStrings");

            ConcatWideStrings method = (ConcatWideStrings)Marshal.GetDelegateForFunctionPointer(
                func,
                typeof(ConcatWideStrings));
            
            byte[] left_bytes = Encoding.UTF8.GetBytes($"{left}\0");
            byte[] right_bytes = Encoding.UTF8.GetBytes($"{right}\0");

            int left_size = left_bytes.Length;
            IntPtr left_ptr = Marshal.AllocHGlobal(left_size);
            Marshal.Copy(left_bytes, 0, left_ptr, left_bytes.Length);

            int right_size = right_bytes.Length;
            IntPtr right_ptr = Marshal.AllocHGlobal(right_size);
            Marshal.Copy(right_bytes, 0, right_ptr, right_bytes.Length);

            IntPtr result_ptr = IntPtr.Zero;
            int count = method(left_ptr, right_ptr, out result_ptr);

            Marshal.FreeHGlobal(left_ptr);
            Marshal.FreeHGlobal(right_ptr);
            
            string result_str = "";
            byte[] bytes = new byte[count];
            Marshal.Copy(result_ptr, bytes, 0, count);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                result_str = Encoding.Unicode.GetString(bytes);
            }
            else
            {
                result_str = Encoding.UTF32.GetString(bytes);
            }
            DeleteArray(result_ptr); // don't forget to release unmanaged memory

            return result_str;
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