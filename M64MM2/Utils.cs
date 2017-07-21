﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace M64MM2
{
    static class Utils
    {
        public static long BaseAddress;
        public static bool IsEmuOpen => (emuProcess != null && !emuProcess.HasExited);

        static Process emuProcess;
        static IntPtr emuProcessHandle;
        const int PROCESS_ALL_ACCESS = 0x01F0FF;

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(Keys vKey);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, long nSize, ref long lpNumberOfBytesRead);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, long nSize, ref long lpNumberOfBytesWritten);


        #region Reading
        public static byte[] ReadBytes(long address, long size)
        {
            IntPtr ptr = new IntPtr(address);
            byte[] buffer = new byte[size];
            long bytesRead = 0;

            ReadProcessMemory(emuProcessHandle, ptr, buffer, size, ref bytesRead);
            return buffer;
        }

        public static ushort ReadUShort(long address)
        {
            long size = sizeof(ushort);
            byte[] buffer = ReadBytes(address, size);
            ushort value = BitConverter.ToUInt16(buffer, 0);
            return value;
        }

        public static uint ReadUInt(long address)
        {
            long size = sizeof(uint);
            byte[] buffer = ReadBytes(address, size);
            uint value = BitConverter.ToUInt32(buffer, 0);
            return value;
        }

        public static ulong ReadULong(long address)
        {
            long size = sizeof(ulong);
            byte[] buffer = ReadBytes(address, size);
            ulong value = BitConverter.ToUInt64(buffer, 0);
            return value;
        }
        #endregion


        #region Writing
        public static void WriteBytes(long address, byte[] data)
        {
            IntPtr ptr = new IntPtr(address);
            long size = data.LongLength;
            long bytesWritten = 0;

            WriteProcessMemory(emuProcessHandle, ptr, data, size, ref bytesWritten);
        }

        public static void WriteUShort(long address, ushort data)
        {
            byte[] buffer = BitConverter.GetBytes(data);
            WriteBytes(address, buffer);
        }

        public static void WriteUInt(long address, uint data)
        {
            byte[] buffer = BitConverter.GetBytes(data);
            WriteBytes(address, buffer);
        }

        public static void WriteULong(long address, ulong data)
        {
            byte[] buffer = BitConverter.GetBytes(data);
            WriteBytes(address, buffer);
        }
        #endregion


        public static void FindEmuProcess()
        {
            foreach (Process proc in Process.GetProcesses())
            {
                if (proc.ProcessName.Contains("Project64"))
                {
                    emuProcess = proc;
                    emuProcessHandle = OpenProcess(PROCESS_ALL_ACCESS, false, emuProcess.Id);
                    break;
                }
            }
        }

        public static void FindBaseAddress()
        {
            uint value = 0;

            long start = 0x20000000;
            long stop = 0x60000000;
            long step = 0x10000;

            //Check if the old base address is still valid
            if (BaseAddress > 0)
            {
                value = ReadUInt(BaseAddress);

                if (value == 0x3C1A8032) return;
            }

            for (long scanAddress = start; scanAddress < stop - step; scanAddress += step)
            {
                value = ReadUInt(scanAddress);

                if (value == 0x3C1A8032)
                {
                    BaseAddress = scanAddress;
                    return;
                }
            }

            //If we don't find anything, reset the base address to 0
            BaseAddress = 0;
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }

        public static byte[] SwapEndian(byte[] array, int wordSize)
        {
            for (int x = 0; x < array.Length / wordSize; x++)
            {
                Array.Reverse(array, x * wordSize, wordSize);
            }

            return array;
        }


        public static bool GetKey(Keys vKey)
        {
            return 0 != GetAsyncKeyState(vKey);
        }
    }
}
