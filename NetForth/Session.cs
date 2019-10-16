using System;
using System.Runtime.InteropServices;

namespace NetForth
{
	internal class Session : IDisposable
    {
        internal static readonly ForthStack<int> Stack = new ForthStack<int>();
        internal static IntPtr ForthMemory;
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
            ForthMemory = Marshal.AllocHGlobal(CbMemory);
            FreeOffset = 0;
            Vocabulary.Init();
            ReturnStack = new ForthStack<int>();
        }

        private void ReleaseUnmanagedResources()
        {
			Marshal.FreeHGlobal(ForthMemory);
            ForthMemory = (IntPtr) 0;
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
