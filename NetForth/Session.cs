using System;
using System.Runtime.InteropServices;

namespace NetForth
{
	internal class FSession : IDisposable
    {
        internal static IntPtr Memory;
        internal static int CbMemory;
        internal static Primitive RunningPrimitive;

        public FSession(int cbMemory = 5000)
        {
            CbMemory = cbMemory;
            Memory = Marshal.AllocHGlobal(CbMemory);
        }

        private void ReleaseUnmanagedResources()
        {
			Marshal.FreeHGlobal(Memory);
            Memory = (IntPtr) 0;
		}

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~FSession()
        {
            ReleaseUnmanagedResources();
        }
    }
}
