using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using ClassByKamil;
using Microsoft.Win32;
using System.IO;
using System.Windows.Forms;

namespace Orion
{
    class Define
    {
        private static SafeFileHandle Handle = null;
        private static string linkDriver = "\\\\.\\"; // не меняеть
        private static string nameDriver = "MBAMService"; // не меняеть

        public static uint CODE(uint DeviceType, uint Function, uint Nethod, uint Accese)
        {  return (((DeviceType) << 16) | ((Accese) << 14) | ((Function)) << 2 | (Nethod)); }

        private static uint Write = CODE(0x1692, 0x522, 0, 3); // не меняет
        private static uint Read = CODE(0x1692, 0x633, 0, 3); // не менять


        //Загружаем драйвер // Выгружаем драйвер  \\ Для вызова функций //
        public static SYS.NTSTATUS EnsureLoaded(string path = "")
        {
            if (Define.Handle != null && Define.Handle.IsInvalid == false)
                return SYS.NTSTATUS.Success;

            Define.Handle = SYS.CreateFile(Define.linkDriver, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
            if (Define.Handle == null || Define.Handle.IsInvalid)
                return Define.Reload(path);

            return SYS.NTSTATUS.Success;
        }

        public static SYS.NTSTATUS Unload()
        {
            if (Define.Handle != null && Define.Handle.IsInvalid == false)
            {  Define.Handle.Close(); Define.Handle = null;  }
            return Define.UnloadDriver(Define.nameDriver);
        }


        //Чтение // Запись
        struct _MEMORY
        {
            public Int64 pBuffer;
            public Int64 pAdress;
            public Int64 iSize;
            public uint pId;
            public byte writes;
        };
       
        public static SYS.NTSTATUS mRead(int pid, Int64 addr, Int64 size, object buffer)
        {
            if (Define.Handle == null || Define.Handle.IsInvalid)
                return SYS.NTSTATUS.DeviceDoesNotExist;

            var info = new _MEMORY();
            var pinned = GCHandle.Alloc(buffer, GCHandleType.Pinned);

            info.pId = (uint)pid;
            info.iSize = size;
            info.writes = 0;
            info.pBuffer = pinned.AddrOfPinnedObject().ToInt64();
            info.pAdress = addr;

            int bytes = 0;
            if (!SYS.DeviceIoControl(Define.Handle, Read, info, (uint)Marshal.SizeOf(info), null, 0, ref bytes, IntPtr.Zero))
            {
                return (SYS.NTSTATUS)Marshal.GetLastWin32Error();
            }

            return SYS.NTSTATUS.Success;
        }

        public static SYS.NTSTATUS mWrite(int pid, Int64 addr, Int64 size, object buffer)
        {
            if (Define.Handle == null || Define.Handle.IsInvalid)
                return SYS.NTSTATUS.DeviceDoesNotExist;
            

            var info = new _MEMORY();
            var pinned = GCHandle.Alloc(buffer, GCHandleType.Pinned);

            info.pId = (uint)pid;
            info.iSize = size;
            info.writes = 0;
            info.pBuffer = pinned.AddrOfPinnedObject().ToInt64();
            info.pAdress = addr;

            int bytes = 0;
            if (!SYS.DeviceIoControl(Define.Handle, Write, info, (uint)Marshal.SizeOf(info), null, 0, ref bytes, IntPtr.Zero))
            {
                return (SYS.NTSTATUS)Marshal.GetLastWin32Error();
            }

            return SYS.NTSTATUS.Success;
        }


        // Загрузка // Выгрузка // Перезагрузка  - с помощью реестра \\ Сами функции //
        private static SYS.NTSTATUS LoadDriver(string svcName, string path)
        {
            string regPath = "CurrentControlSet\\Services\\" + svcName; RegistryKey svcKey = Registry.LocalMachine.CreateSubKey("SYSTEM\\" + regPath);
            svcKey.SetValue("ImagePath", "\\??\\" + path); svcKey.SetValue("Type", 1);
            SYS.UNICODE_STRING uRegPath = new SYS.UNICODE_STRING();
            bool enabled; var status = SYS.RtlAdjustPrivilege(SYS.SeLoadDriverPrivilege, true, SYS.ADJUST_PRIVILEGE_TYPE.AdjustCurrentProcess, out enabled);
            SYS.RtlInitUnicodeString(ref uRegPath, "\\Registry\\Machine\\SYSTEM\\" + regPath);
            return SYS.NtLoadDriver(ref uRegPath);
        }

        private static SYS.NTSTATUS UnloadDriver(string svcName)
        {
            bool enabled; string regPath = "CurrentControlSet\\Services\\" + svcName;
            SYS.UNICODE_STRING uRegPath = new SYS.UNICODE_STRING();
            SYS.NTSTATUS status = SYS.RtlAdjustPrivilege(SYS.SeLoadDriverPrivilege, true,   SYS.ADJUST_PRIVILEGE_TYPE.AdjustCurrentProcess, out enabled);
            SYS.RtlInitUnicodeString(ref uRegPath, "\\Registry\\Machine\\SYSTEM\\" + regPath);  status = SYS.NtUnloadDriver(ref uRegPath); Registry.LocalMachine.DeleteSubKeyTree("SYSTEM\\" + regPath, false);
            return status;
        }

        private static SYS.NTSTATUS Reload(string path)
        {
            Define.Unload();
            if (String.IsNullOrWhiteSpace(path)) {
                return SYS.NTSTATUS.ObjectPathNotFound; }
            SYS.NTSTATUS status = Define.LoadDriver(Define.nameDriver, path);
            if (status >= 0)
            {
                Define.Handle = SYS.CreateFile(Define.linkDriver, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
                if (Define.Handle == null || Define.Handle.IsInvalid)  {
                    return (SYS.NTSTATUS)Marshal.GetLastWin32Error();} } return SYS.NTSTATUS.Success;
        }
    }
}
