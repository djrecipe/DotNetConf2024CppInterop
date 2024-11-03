using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal class MyDataMarshaller : ICustomMarshaler
    {
        private static MyDataMarshaller static_instance = null;
        public static ICustomMarshaler GetInstance(string cookie)
        {
            if (static_instance == null)
            {
                return static_instance = new MyDataMarshaller();
            }
            return static_instance;
        }
        public MyDataMarshaller()
        {

        }
        public void CleanUpNativeData(IntPtr pNativeData)
        {
            Marshal.FreeHGlobal(pNativeData);
        }

        public unsafe int GetNativeDataSize()
        {
            return sizeof(MyData);
        }
        public void CleanUpManagedData(object ManagedObj)
        {
        }

        public unsafe IntPtr MarshalManagedToNative(object ManagedObj)
        {
            var data = (MyDataClass)ManagedObj;
            MyData data_struct;
            data_struct.Value1 = data.Value1;
            data_struct.Value2 = data.Value2;
            data_struct.ArrayCount = data.MoreValues.Length;
            data_struct.ArrayValues = Marshal.AllocHGlobal(data.MoreValues.Length);
            Marshal.Copy(data.MoreValues, 0, data_struct.ArrayValues, data.MoreValues.Length);
            data.instance = data_struct;
            data.ptr = Marshal.AllocHGlobal(sizeof(MyData));
            Marshal.StructureToPtr<MyData>(data_struct, data.ptr, true);
            return data.ptr;
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            MyData data_struct = Marshal.PtrToStructure<MyData>(pNativeData);
            MyDataClass result = new MyDataClass();
            result.Value1  = data_struct.Value1;
            result.Value2 = data_struct.Value2;
            int[] more_values = new int[data_struct.ArrayCount];
            Marshal.Copy(data_struct.ArrayValues, more_values, 0, data_struct.ArrayCount);
            result.MoreValues = more_values;
            return result;
        }
    }
    internal class MyDataClass
    {
        internal MyData instance;
        internal IntPtr ptr;

        public int Value1 { get; set; }
        public double Value2 { get; set; }
        public int[] MoreValues { get; set; }

        public MyDataClass()
        {
            instance.ArrayValues = IntPtr.Zero;
            ptr = IntPtr.Zero;
        }

    }
}
