using System;
using System.Runtime.InteropServices;

namespace NetForth
{
	internal class Session : IDisposable
    {
        internal static IntPtr Memory;
        internal static int CbMemory;
        internal static int FreeOffset;
        internal static string LastDefinedWord;
        internal static ForthStack<int> ReturnStack;
#if SMALLSTRINGS
        public const int StringLengthSize = 1;
#else
        public const int StringLengthSize = sizeof(int);
#endif

		public Session(int cbMemory = 5000)
        {
            CbMemory = cbMemory;
            Memory = Marshal.AllocHGlobal(CbMemory);
            FreeOffset = 0;
            Vocabulary.Init();
            ReturnStack = new ForthStack<int>();
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
