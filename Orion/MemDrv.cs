using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Orion
{
    class MemDrv
    {
        private static int pID = Process.GetProcessesByName("Game")[0].Id;
        private static int redix;
        private static int wedix;

        private static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {

            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }
        }
        private static byte[] StructureToByteArray(object obj)
        {
            int length = Marshal.SizeOf(obj);
            byte[] array = new byte[length];
            IntPtr pointer = Marshal.AllocHGlobal(length);
            Marshal.StructureToPtr(obj, pointer, true);
            Marshal.Copy(pointer, array, 0, length);
            Marshal.FreeHGlobal(pointer);
            return array;
        }


        //mRead(int pid, Int64 addr, Int64 size, object buffer)
        public static T ReadMemory<T>(int address) where T : struct
        {
            int ByteSize = Marshal.SizeOf(typeof(T));
            byte[] buffer = new byte[ByteSize];
            Define.mRead((int)pID, address, buffer.Length, buffer);
            return ByteArrayToStructure<T>(buffer);
        }

        //WriteMem(DWORD pid, uint64_t base, uint64_t size, PVOID buffer)
        public static void WriteMemory<T>(int address, object Value) where T : struct
        {
            byte[] buffer = StructureToByteArray(Value);
            Define.mWrite((int)pID, address, buffer.Length, buffer);
        }
    }
}
