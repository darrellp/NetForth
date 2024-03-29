﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using NetForth.Primitives;

namespace NetForth
{
	internal class Session : IDisposable
    {
		#region Statics
        public static readonly ForthStack<int> ReturnStack = new ForthStack<int>();
		public static readonly ForthStack<int> Stack = new ForthStack<int>();
        public static IntPtr ForthMemory;
        public static IntPtr StackStringMemory;
        public static int StackStringMemoryCapacity;
        public static int FreeOffset;
        public static readonly List<Evaluable> EvaluableVals = new List<Evaluable>();
        public static readonly List<object> DotNetObjects = new List<object>();

        internal static int CbMemory;
        internal static string LastDefinedWord;
        internal static bool IsTesting;
#if SMALLSTRINGS
        // ReSharper disable once ConvertToConstant.Global
        public static readonly int StringLengthSize = 1;
#else
		// ReSharper disable once ConvertToConstant.Global
		public static readonly int StringLengthSize = sizeof(int);
#endif
		#endregion

		#region Constructor
		public Session(int cbMemory = 5000)
        {
            CbMemory = cbMemory;
            ForthMemory = Marshal.AllocHGlobal(CbMemory);
            FreeOffset = 0;
            StackStringMemoryCapacity = 0;

			Vocabulary.Init();

			ReturnStack.Clear();
            EvaluableVals.Clear();
            DotNetObjects.Clear();
            RootPrims.AddRoot();

            StackTrace stackTrace = new StackTrace();           // get call stack
            StackFrame[] stackFrames = stackTrace.GetFrames();  // get method calls (frames)

            // ReSharper disable once PossibleNullReferenceException
            // write call stack method names
            foreach (var stackFrame in stackFrames)
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

		#region .NET interaction
        public static void AddDotNetFn(string name, Delegate myDelegate)
        {
            Vocabulary.CurrentVocabulary.AddDefinition(name, new DotNetDelegatePrimitive(myDelegate));
        }

        public static int SaveManagedObject(Object obj)
        {
            var iRet = DotNetObjects.Count;
            DotNetObjects.Add(obj);
            return iRet;
        }

        internal static object GetManagedObject(int index)
        {
            if (index == -1)
            {
                return null;
            }
            if (index > DotNetObjects.Count)
            {
                throw new NfException("Out of range in DotNetObjects list");
            }

            return DotNetObjects[index];

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
