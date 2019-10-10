using System;
using System.Diagnostics;

namespace NetForth
{
    internal static unsafe class Memory
    {
        private static int _freeOffset;

        internal static int Allocate(int cb = 4)
        {
#if MEMORYCHECK
			if (_freeOffset + cb > FSession.CbMemory)
            {
                throw new NfException("Out of memory");
            }
#endif
			var ret = (int)FSession.Memory + _freeOffset;
            _freeOffset += cb;
            return ret;
        }

        internal static int Create()
        {
            return Allocate(0);
        }

        internal static int FetchInt(int address)
        {
            CheckBlock((IntPtr)address);
            return *(int *)address;
        }

        internal static void StoreInt(int address, int value)
        {
            CheckBlock((IntPtr)address);
            *(int *)address = value;
        }

        internal static short FetchShort(int address)
        {
            CheckBlock((IntPtr)address, 2);
            return *(short*)address;
        }

        internal static void StoreShort(int address, short value)
        {
            CheckBlock((IntPtr)address, 2);
            *(short*)address = value;
        }

        internal static ushort FetchUShort(int address)
        {
            CheckBlock((IntPtr)address, 2);
            return *(ushort*)address;
        }

        internal static void StoreUShort(int address, ushort value)
        {
            CheckBlock((IntPtr)address, 2);
            *(ushort*)address = value;
        }

		internal static string FetchString(int address, int length)
		{
			CheckBlock((IntPtr)address, length);
			char[] charArray = new char[length];
			var pch = (char*)address;
			for (int i = 0; i < length; i++)
			{
				charArray[i] = *pch++;
			}
			return new string(charArray);
		}

		internal static void StoreString(int address, string value)
		{
			CheckBlock((IntPtr)address, value.Length + sizeof(int));
			var pch = (char*)address;
			char[] charArray = value.ToCharArray();
			for (int i = 0; i < value.Length; i++)
			{
				*(pch + i) = charArray[i];
			}
		}

		public static string FetchCString(int address)
		{
			CheckBlock((IntPtr)address);
			int length = FetchInt(address);
			return FetchString(address + sizeof(int), length);
		}

		internal static void StoreCString(int address, string value)
		{
			CheckBlock((IntPtr)address);
			StoreInt(address, value.Length);
			StoreString(address + sizeof(int), value);
		}
		[Conditional("MEMORYCHECK")]
        private static void  CheckAddress(IntPtr ptr)
        {
            var iPtr = (int) ptr;
            var iMemory = (int) FSession.Memory;

            if (iPtr < iMemory || iPtr >= iMemory + FSession.CbMemory)
            {
                throw new NfException("Invalid memory access");
            }
        }

        [Conditional("MEMORYCHECK")]
        private static void CheckBlock(IntPtr ptr, int cb = 4)
        {
            CheckAddress(ptr);
            CheckAddress(ptr + cb);
        }


	}
}
