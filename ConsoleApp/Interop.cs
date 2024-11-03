using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApp.Interop.Delegates;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ConsoleApp
{
    [StructLayout(LayoutKind.Sequential)]
    struct MyData
    {
        public int Value1;
        public double Value2;
        public int ArrayCount;
        public IntPtr ArrayValues;
    };
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
            internal delegate int DeleteStruct(IntPtr ptr);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate int MyMangledName();
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate int AddStructValues(IntPtr ptr);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate int AddStructValuesCustomMarshaller(
                [MarshalAs(UnmanagedType.CustomMarshaler,
                    MarshalTypeRef = typeof(MyDataMarshaller))]
                MyDataClass data);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate void ThrowUnhandledException();
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate int ThrowCaughtException();
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

        internal static int DeleteArray(IntPtr ptr)
        {
            IntPtr func = NativeLibrary.GetExport(library, "DeleteArray");

            DeleteArray method = (DeleteArray)Marshal.GetDelegateForFunctionPointer(
                func,
                typeof(DeleteArray));

            return method(ptr);
        }

        internal static int DeleteStruct(IntPtr ptr)
        {
            IntPtr func = NativeLibrary.GetExport(library, "DeleteStruct");

            DeleteStruct method = (DeleteStruct)Marshal.GetDelegateForFunctionPointer(
                func,
                typeof(DeleteStruct));

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
            // load function
            IntPtr func = NativeLibrary.GetExport(library, "ConcatWideStrings");
            ConcatWideStrings method = (ConcatWideStrings)Marshal.GetDelegateForFunctionPointer(
                func,
                typeof(ConcatWideStrings));
            
            // get null-terminated bytes
            byte[] left_bytes = Encoding.UTF8.GetBytes($"{left}\0");
            byte[] right_bytes = Encoding.UTF8.GetBytes($"{right}\0");

            // copy bytes to unmanaged memory
            int left_size = left_bytes.Length;
            IntPtr left_ptr = Marshal.AllocHGlobal(left_size);
            Marshal.Copy(left_bytes, 0, left_ptr, left_bytes.Length);
            // copy bytes to unmanaged memory
            int right_size = right_bytes.Length;
            IntPtr right_ptr = Marshal.AllocHGlobal(right_size);
            Marshal.Copy(right_bytes, 0, right_ptr, right_bytes.Length);

            // invoke method
            IntPtr result_ptr = IntPtr.Zero;
            int count = method(left_ptr, right_ptr, out result_ptr);

            // free parameters
            Marshal.FreeHGlobal(left_ptr);
            Marshal.FreeHGlobal(right_ptr);
            
            // get result string
            string result_str = "";
            byte[] bytes = new byte[count];
            Marshal.Copy(result_ptr, bytes, 0, count);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                result_str = Encoding.Unicode.GetString(bytes);
            else
                result_str = Encoding.UTF32.GetString(bytes);
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

        public static void ThrowUnhandledException()
        {
            IntPtr func = NativeLibrary.GetExport(library, "ThrowUnhandledException");

            ThrowUnhandledException method = (ThrowUnhandledException)Marshal.GetDelegateForFunctionPointer(
                func,
                typeof(ThrowUnhandledException));

            method();
        }

        public static void ThrowCaughtException()
        {
            IntPtr func = NativeLibrary.GetExport(library, "ThrowCaughtException");

            ThrowCaughtException throwcaughtexception = (ThrowCaughtException)Marshal.GetDelegateForFunctionPointer(
                func,
                typeof(ThrowCaughtException));

            int result = throwcaughtexception();
            if (result < 0)
                throw new Exception("Native exception!");
        }

        public unsafe static int AddValues(int value1, double value2, int[] more_values)
        {
            IntPtr func = NativeLibrary.GetExport(library, "AddStructValues");
            if (func == IntPtr.Zero)
                throw new Exception("Failed to find function with name 'MyMangledName'");

            AddStructValues method = (AddStructValues)Marshal.GetDelegateForFunctionPointer(
                func,
                typeof(AddStructValues));

            MyData struct_data;
            struct_data.Value1 = value1;
            struct_data.Value2 = value2;
            struct_data.ArrayCount = more_values.Length;
            struct_data.ArrayValues = Marshal.AllocHGlobal(sizeof(int) * more_values.Length);
            Marshal.Copy(more_values, 0, struct_data.ArrayValues, struct_data.ArrayCount);

            IntPtr struct_ptr = Marshal.AllocHGlobal(sizeof(MyData));
            Marshal.StructureToPtr<MyData>(struct_data, struct_ptr, false);
            
            // invoke method
            int result = method(struct_ptr);

            // free memory
            Marshal.FreeHGlobal(struct_data.ArrayValues);
            Marshal.FreeHGlobal(struct_ptr);

            return result;
        }

        public static int AddValues(MyDataClass data)
        {
            IntPtr func = NativeLibrary.GetExport(library, "AddStructValues");

            AddStructValuesCustomMarshaller method = (AddStructValuesCustomMarshaller)
                Marshal.GetDelegateForFunctionPointer(
                func, typeof(AddStructValuesCustomMarshaller));

            return method(data);
        }
    }
}