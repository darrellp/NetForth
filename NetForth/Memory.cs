using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace NetForth
{
    internal static unsafe class Memory
    {
		#region Memory operations
		internal static int Allocate(int cb = 4)
        {
#if MEMORYCHECK
			if (Session.FreeOffset + cb > Session.CbMemory)
            {
                throw new NfException("Out of memory");
            }
#endif
			var ret = (int)Session.ForthMemory + Session.FreeOffset;
            Session.FreeOffset += cb;
            return ret;
        }

        internal static int Here()
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

        internal static void StoreChar(int address, char ch)
        {
            CheckBlock((IntPtr)address, 2);
            *(char*) address = ch;
        }

		internal static char FetchChar(int address)
        {
            CheckBlock((IntPtr)address, 2);
            return *(char*)address;
        }

        internal static void StoreByte(int address, byte b)
        {
            CheckBlock((IntPtr)address, 1);
            *(byte*) address = b;
        }

        internal static byte FetchByte(int address)
		{ 
            CheckBlock((IntPtr)address, 1);
            return *(byte *)address;
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

        internal static string FetchCString(int address)
        {
            var length = 0;

            switch (Session.StringLengthSize)
            {
                case 1:
                    length = FetchByte(address);
                    break;

                case sizeof(int):
                    length = FetchInt(address);
                    break;
            }

            return FetchString(address + Session.StringLengthSize, length);
        }

		internal static string FetchString(int address, int length)
		{
			CheckBlock((IntPtr)address, length);
			var charArray = new char[length];
			var pch = (char*)address;
			for (var i = 0; i < length; i++)
			{
				charArray[i] = *pch++;
			}
			return new string(charArray);
		}

		internal static void StoreString(int address, string value)
		{
			CheckBlock((IntPtr)address, value.Length);
			var pch = (char*)address;
			char[] charArray = value.ToCharArray();
			for (int i = 0; i < value.Length; i++)
			{
				*(pch + i) = charArray[i];
			}
		}

        internal static void StoreCString(int address, string value)
		{
			CheckBlock((IntPtr)address);
            switch (Session.StringLengthSize)
            {
                case 1:
                    StoreByte(address, (byte)value.Length);
                    break;

                case sizeof(int):
                    StoreInt(address, value.Length);
                    break;
            }
			StoreString(address + Session.StringLengthSize, value);
		}

		[Conditional("MEMORYCHECK")]
        private static void  CheckAddress(IntPtr ptr)
        {
            var iPtr = (int) ptr;
            var iPtrStack = (int) Session.StackStringMemory;
            var iMemory = (int) Session.ForthMemory;

            if ((iPtr < iPtrStack || iPtr >= iPtrStack + Session.StackStringMemoryCapacity) && (iPtr < iMemory || iPtr >= iMemory + Session.CbMemory))
            {
                throw new NfException("Invalid memory access");
            }
        }

        [Conditional("MEMORYCHECK")]
        private static void CheckBlock(IntPtr ptr, int cb = 4)
        {
            CheckAddress(ptr);
            CheckAddress(ptr + cb - 1);
        }

        static readonly UnicodeEncoding unicode = new UnicodeEncoding();
		internal static string BytesToString(int p, int cch)
        {
            return unicode.GetString((byte*) p, cch * 2);
        }
		#endregion

        public static int AllocateStackString(int stringLengthSize)
        {
            if (Session.StackStringMemoryCapacity < stringLengthSize)
            {
                if (Session.StackStringMemory != null)
                {
                    Marshal.FreeHGlobal(Session.StackStringMemory);
                }

                Session.StackStringMemoryCapacity = stringLengthSize;
                Session.StackStringMemory = Marshal.AllocHGlobal(stringLengthSize);
            }

            return (int)Session.StackStringMemory;
        }
    }
}
