using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        internal static bool IsTesting;
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

            StackTrace stackTrace = new StackTrace();           // get call stack
            StackFrame[] stackFrames = stackTrace.GetFrames();  // get method calls (frames)

            // write call stack method names
            foreach (StackFrame stackFrame in stackFrames)
            {
                var name = stackFrame.GetMethod().Module.Name;

				IsTesting = name.Contains("NetForthTests");
                if (IsTesting)
                {
                    return;
                }
            }
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
