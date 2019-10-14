using System;
using System.Runtime.InteropServices;

namespace NetForth
{
	internal class Session : IDisposable
    {
        internal static IntPtr Memory;
        internal static int CbMemory;
        internal static Primitive RunningPrimitive;
        internal static LookAhead RunningLookAhead;
        internal static int FreeOffset;
        internal static string LastDefinedWord;

		public Session(int cbMemory = 5000)
        {
            CbMemory = cbMemory;
            Memory = Marshal.AllocHGlobal(CbMemory);
            FreeOffset = 0;
            Vocabulary.Init();
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

        ~Session()
        {
            ReleaseUnmanagedResources();
        }
    }
}
