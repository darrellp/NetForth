using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NetForth
{
	internal class Session : IDisposable
    {
		#region Statics
		internal static readonly ForthStack<int> Stack = new ForthStack<int>();
        internal static IntPtr ForthMemory;
        internal static int CbMemory;
        internal static int FreeOffset;
        internal static string LastDefinedWord;
        internal static ForthStack<int> ReturnStack;
#if SMALLSTRINGS
        public static readonly int StringLengthSize = 1;
#else
        public static readonly int StringLengthSize = sizeof(int);
#endif
        internal static Evaluable[] EvaluableVals;
        internal static Dictionary<string, int> MapWordToIndex;
        internal static int NextEvalSlot;
		#endregion

		#region Constructor
		public Session(int cbMemory = 5000, int cEvalSlots = 500)
        {
            CbMemory = cbMemory;
            ForthMemory = Marshal.AllocHGlobal(CbMemory);
            FreeOffset = 0;
            Vocabulary.Init();
            ReturnStack = new ForthStack<int>();
            EvaluableVals = new Evaluable[cEvalSlots];
            MapWordToIndex = new Dictionary<string, int>();
			NextEvalSlot = 0;
        }
		#endregion

		#region IDispose
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
		#endregion
	}
}
