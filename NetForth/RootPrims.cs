using System;
using System.Collections.Generic;
using System.IO;
using NetForth.Primitives;
using NetForth.WordInterpreters;
using static NetForth.Evaluable;
using static NetForth.Session;

// ReSharper disable InconsistentNaming

namespace NetForth
{
#pragma warning disable IDE1006 // Naming Styles
	internal static class RootPrims
    {
		#region Setting up root vocabulary
		internal static void AddRoot()
        {
            var rootPrimitives = new Dictionary<string, Evaluable>()
            {
                // MATH --------------------------------------------------------
                {"+", new Primitive(plus, "+")},
                {"-", new Primitive(minus, "-")},
				{"*", new Primitive(times, "*")},
				{"/", new Primitive(divide, "/")},
                {"um*", new Primitive(umStar, "um*")},
                {"*/", new Primitive(timesDiv, "*/")},
                {"mod", new Primitive(mod, "mod")},
                {"um/mod", new Primitive(umMod, "um/mod")},
                {"negate", new Primitive(negate, "negate")},
                {"abs", new Primitive(abs, "abs")},
                {"min", new Primitive(min, "min")},
                {"max", new Primitive(max, "max")},
                {"1-", new Primitive(oneMinus, "1-") },
                {"1+", new Primitive(onePlus, "1-") },
                // STACK --------------------------------------------------------
                {"dup", new Primitive(dup, "dup")},
                {"?dup", new Primitive(qDup, "?dup")},
                {"drop", new Primitive(drop, "drop")},
                {"swap", new Primitive(swap, "swap")},
                {"over", new Primitive(over, "over")},
				{"nip", new Primitive(nip, "nip")},
                {"tuck", new Primitive(tuck, "tuck")},
                {"rot", new Primitive(rot, "rot")},
				{"-rot", new Primitive(minusRot, "-rot")},
                {"pick", new Primitive(pick, "pick")},
                {"2dup", new Primitive(dup2, "2dup")},
                {"2drop", new Primitive(drop2, "2drop")},
                {"2swap", new Primitive(swap2, "2swap")},
                {"2over", new Primitive(over2, "2over")},
                // COMMENTS ------------------------------------------------------
                {"\\", new LookAhead(lineComment, "\\")},
                {"(", new LookAhead(mlComment, "(", true)},
                // COMPARISONS ---------------------------------------------------
                {"<", new Primitive(lt, "<") },
                {">", new Primitive(gt, ">") },
                {"<=", new Primitive(leq, "<=") },
                {">=", new Primitive(geq, ">=") },
                {"u<", new Primitive(ult, "u<") },
                {"u>", new Primitive(ugt, "u>") },
                {"=", new Primitive(eq, "=") },
                {"<>", new Primitive(neq, "<>") },
                {"0=", new Primitive(zeq, "0=") },
                {"0<>", new Primitive(zneq, "0<>") },
                {"0>", new Primitive(zgt, "0>") },
                {"0<", new Primitive(zlt, "0<") },
                // LOGICAL OPERATIONS --------------------------------------------
                {"and", new Primitive(and, "and") },
                {"or", new Primitive(or, "or") },
                {"xor", new Primitive(xor, "xor") },
                {"invert", new Primitive(invert, "invert") },
                {"lshift", new Primitive(lshift, "lshift") },
                {"rshift", new Primitive(rshift, "rshift") },
                // DEFINING -------------------------------------------------------
                {":", new LookAhead(define, ":") },
                {"constant", new LookAhead(constant, "constant") },
                {"variable", new LookAhead(variable, "variable") },
                {"value", new LookAhead(value, "value") },
                {"'", new LookAhead(tick, "'") },
                {"execute", new LookAhead(execute, "execute") },
                // MEMORY ---------------------------------------------------------
                {"@", new Primitive(fetch, "@") },
                {"!", new Primitive(store, "!") },
                {"c@", new Primitive(cFetch, "c@") },
                {"c!", new Primitive(cStore, "c!") },
                {"w@", new Primitive(wFetch, "w@") },
                {"w!", new Primitive(wStore, "w!") },
                {"b@", new Primitive(bFetch, "b@") },
                {"b!", new Primitive(bStore, "b!") },
                {"+!", new Primitive(memAdd, "+!") },
                {"cells", new Primitive(cells, "cells") },
                {"chars", new Primitive(chars, "chars") },
                // FLOW OF CONTROL ---------------------------------------------------
                {"if", new Compilable(iffn) },
                {"do", new Compilable(dofn) },
				{"?do", new Compilable(questDofn) },
                {"begin", new Compilable(begin) },
				{"i", new Primitive(i, "i") },
				{"j", new Primitive(j, "j") },
				{"leave", new ThrowPrimitive(leave, "leave") },
				{"?leave", new ThrowPrimitive(condLeave, "?leave") },
                {"exit", new ThrowPrimitive(exit, "exit") },
                // STRINGS -------------------------------------------------------------
				{"c\"", new Compilable(countedString) },
                {"s\"", new Compilable(stackString) },
                {"[char]", new LookAhead(fromChar, "[char]", true) },
                {"char", new LookAhead(fromChar, "char") },
                {"strhead", new Primitive(strhead, "strhead") },
                {"count", new Primitive(count, "count") },
                {"cstrTo.Net", new Primitive(cstrToDNet, "cstrTo.Net") },
                // MEMORY ALLOTMENT -----------------------------------------------------
                {"create", new LookAhead(create, "create") },
                {",", new Primitive(comma, ",") },
                {"c,", new Primitive(charComma, "c,") },
                {"here", new Primitive(here, "here") },
                {"allot", new Primitive(allot, "allot") },
                // RETURN STACK ---------------------------------------------------------
                {">r", new Primitive(ontoR, ">r") },
                {"r>", new Primitive(fromR, "r>") },
                {"r@", new Primitive(copyR, "r@") },
                {"2>r", new Primitive(ontoR2, "2>r") },
                {"2r>", new Primitive(fromR2, "2r>") },
                {"2r@", new Primitive(copyR2, "2r@") },
                // I/O ------------------------------------------------------------------
                {".", new Primitive(dot, ".")},
                {".s", new Primitive(dotS, ".s")},
                {"emit", new Primitive(emit, "emit") },
                {"cr", new Primitive(cr, "cr") },
                {"page", new Primitive(page, "page") },
                {"type", new Primitive(type, "type") },
                {"key", new Primitive(key, "key") },
                {"included", new Primitive(included, "included") },
                // .NET -----------------------------------------------------------------
                {"defmeth", new LookAhead(defmeth, "defmeth") },
                {"defcnst", new LookAhead(defcnst, "defcnst") },
                {"defindx", new LookAhead(defindx, "defindx") },
                {"defstat", new LookAhead(defstat, "defstat") },

				{"prop", new LookAhead(prop, "prop") },
                {"sprop", new LookAhead(sprop, "sprop") },
                {"isnull", new Primitive(isnull, "isnull") },
                {"null", new Primitive(pushNull, "null") },
                // Create a .net string and leave it's token on the stack
                {"n\"", new Compilable(netString) },
                // Create a type from the name and leave it's token on the stack
                // If it's a generic type then the types for it's generic arguments
                // should be pushed on the stack ahead when the type is created.
                {"t\"", new Compilable(netType) },
			};

            Vocabulary.AddVocabulary(new Vocabulary(rootPrimitives, "Root"));
            DefineNetTypes();
        }

        private static void DefineNetTypes()
        {
            // Defining some standard .NET types
            Vocabulary.CurrentVocabulary.AddDefinition("tstring", new IntPrim(SaveManagedObject(typeof(string)), "tstring"));
            Vocabulary.CurrentVocabulary.AddDefinition("tbyte", new IntPrim(SaveManagedObject(typeof(byte)), "tbyte"));
            Vocabulary.CurrentVocabulary.AddDefinition("tshort", new IntPrim(SaveManagedObject(typeof(short)), "tshort"));
            Vocabulary.CurrentVocabulary.AddDefinition("tint", new IntPrim(SaveManagedObject(typeof(int)), "tint"));
            Vocabulary.CurrentVocabulary.AddDefinition("tlong", new IntPrim(SaveManagedObject(typeof(long)), "long"));
        }
        #endregion

		#region .NET Interaction
        private static void defmeth(Tokenizer tokenizer)
        {
			// ptype0 ptype1 ... cParms objType "dnName fName" => new method primitive named "fName"
            // where dnName is the actual name of the .NET function and fName is the name we use in
            // Forth.  This is necessary due to .NET's overloading.  Each overload requires a different
            // name in Forth.
			CallAction.CreateMethod(tokenizer);
        }

        private static void defstat(Tokenizer tokenizer)
        {
			// ptype0 ptype1 ... cParms staticType "dnName fName" => new method primitive named "fName"
			CallAction.CreateStaticMethod(tokenizer);
        }

		private static void defcnst(Tokenizer tokenizer)
        {
			// ptype0 ptype1 ... cParms type "fName" => new method primitive named "fName"
			CallAction.CreateConstructor(tokenizer);
        }

		private static void defindx(Tokenizer tokenizer)
        {
			// ptype0 ptype1 ... cParms type "fName" => new method primitive named "fName"
            CallAction.CreateIndexer(tokenizer);
        }

		private static void propHelper(Tokenizer tokenizer, bool isStatic = false)
		{
			var word = tokenizer.NextToken();
            var obj = GetManagedObject(Stack.Pop());
            var property = isStatic ? ((Type)obj).GetProperty(word) : obj.GetType().GetProperty(word);
            if (property == null)
            {
                throw new NfException($"Non-existent property in prop: {word}");
            }

			var result = property.GetValue(obj, null);
            if (result == null)
            {
                Stack.Push(-1);         // Our representation of null
            }
            else
            {
                Stack.Push(result is int intValue ? intValue : SaveManagedObject(result));
            }
        }

        private static void prop(Tokenizer tokenizer)
        {
            propHelper(tokenizer);
        }

        private static void sprop(Tokenizer tokenizer)
        {
            propHelper(tokenizer, true);
        }

        private static void pushNull()
        {
            Stack.Push(-1);
        }

        private static void isnull()
        {
            Stack[-1] = Stack[-1] == -1 ? -1 : 0;
        }
		#endregion

		#region Strings
        // Counted String To .NET
        private static void cstrToDNet()
        {
            Stack[-1] = SaveManagedObject(Memory.FetchCString(Stack[-1]));
        }

		private static void countedString(Tokenizer tokenizer, WordListBuilder wlb)
		{
			FStringAction.FString(tokenizer, wlb);
		}

        private static void stackString(Tokenizer tokenizer, WordListBuilder wlb)
        {
            FStringAction.FString(tokenizer, wlb, StringType.Stack);
        }

        private static void netString(Tokenizer tokenizer, WordListBuilder wlb)
        {
            FStringAction.FString(tokenizer, wlb, StringType.DotNet);
        }

        private static void netType(Tokenizer tokenizer, WordListBuilder wlb)
        {
            FStringAction.FString(tokenizer, wlb, StringType.Type);
        }

		private static void fromChar(Tokenizer tokenizer)
        {
            var word = tokenizer.NextToken();
            Stack.Push(word[0]);
        }
		#endregion

		#region Return Stack
        // The return stack being a part of the actual call stack doesn't make a
        // lot of sense in .Net so it's just a second stack, indistinguishable in all
        // but usage from the Data stack.

        // >r
        private static void ontoR()
        {
            ReturnStack.Push(Stack.Pop());
        }

        // r>
        private static void fromR()
        {
            Stack.Push(ReturnStack.Pop());
        }

        // r@
        private static void copyR()
        {
            Stack.Push(ReturnStack.Peek());
        }

        // 2>r
        private static void ontoR2()
        {
            var top = Stack.Pop();
            ReturnStack.Push(Stack.Pop());
            ReturnStack.Push(top);
        }

        // 2r>
        private static void fromR2()
        {
            var top = ReturnStack.Pop();
            Stack.Push(ReturnStack.Pop());
            Stack.Push(top);
        }

        // 2r@
        private static void copyR2()
        {
            Stack.Push(ReturnStack[-2]);
            Stack.Push(ReturnStack[-1]);
        }
		#endregion

		#region Flow of Control
		private static void iffn(Tokenizer tokenizer, WordListBuilder wlb)
        {
            IfAction.If(tokenizer, wlb);
        }

        private static void dofn(Tokenizer tokenizer, WordListBuilder wlb)
        {
            DoAction.Do(tokenizer, wlb, false);
        }

		private static void questDofn(Tokenizer tokenizer, WordListBuilder wlb)
		{
            DoAction.Do(tokenizer, wlb, true);
		}

		private static void begin(Tokenizer tokenizer, WordListBuilder wlb)
        {
            BeginAction.Begin(tokenizer, wlb);
        }

		private static void i()
		{
			try
			{
				Stack.Push(DoAction.i());
			}
			catch (NfException)
			{
				throw new NfException("Trying to retrieve I outside of loop");
			}
		}

		private static void j()
		{
			try
			{
				Stack.Push(DoAction.j());
			}
			catch (NfException)
			{
				throw new NfException("Trying to retrieve J outside of nested loop");
			}
		}

		private static ExitType leave()
        {
            return ExitType.Leave;
        }

		private static ExitType condLeave()
		{
			if (Stack.Pop() != 0)
			{
				return leave();
			}

            return ExitType.Okay;
        }

        private static ExitType exit()
        {
            return ExitType.Exit;
        }
		#endregion

		#region Memory
		private static void cells()
        {
            Stack[-1] *= 4;
        }

        private static void chars()
        {
            Stack[-1] *= sizeof(char);
        }

        private static void comma()
        {
            Memory.StoreInt(Memory.Allocate(), Stack.Pop());
        }

        private static void charComma()
        {
            Memory.StoreChar(Memory.Allocate(sizeof(char)), (char)Stack.Pop());
        }

        private static void here()
        {
            Stack.Push(Memory.Here());
        }

        private static void allot()
        {
            Memory.Allocate(Stack.Pop());
        }

        private static void constant(Tokenizer tokenizer)
        {
            var word = tokenizer.NextToken().ToLower();
            Vocabulary.CurrentVocabulary.AddDefinition(word, new IntPrim(Stack.Pop(), word));
        }

		private static void variable(Tokenizer tokenizer)
        {
            var word = tokenizer.NextToken().ToLower();
            var address = Memory.Allocate();
            Memory.StoreInt(address, 0);
            Vocabulary.CurrentVocabulary.AddDefinition(word, new IntPrim(address, word));
        }

		private static void value(Tokenizer tokenizer)
        {
            var word = tokenizer.NextToken().ToLower();
            var address = Memory.Allocate();
            Memory.StoreInt(address, Stack.Pop());
            Vocabulary.CurrentVocabulary.AddDefinition(word, new IntPrim(address, word));
        }

        private static void tick(Tokenizer tokenizer)
        {
            var word = tokenizer.NextToken().ToLower();
            var evaluable = Vocabulary.Lookup(word);

            if (evaluable == null)
            {
                throw new NfException("Ticking undefined word");
            }

            Stack.Push(EvaluableVals.Count);
            EvaluableVals.Add(evaluable);
        }

        private static void execute(Tokenizer tokenizer)
        {
            var xt = Stack.Pop();
            if (xt >= EvaluableVals.Count)
            {
                throw new NfException("Invalid execution token");
            }
            EvaluableVals[xt].Eval(tokenizer);
        }

		private static void fetch()
        {
            Stack[-1] = Memory.FetchInt(Stack[-1]);
        }

        private static void store()
        {
            Memory.StoreInt(Stack.Pop(), Stack.Pop());
        }

        private static void cFetch()
        {
            Stack[-1] = Memory.FetchUShort(Stack[-1]);
        }

        private static void cStore()
        {
            Memory.StoreUShort(Stack.Pop(), (ushort)Stack.Pop());
        }

        private static void bFetch()
        {
            Stack[-1] = Memory.FetchByte(Stack[-1]);
        }

        private static void bStore()
        {
            Memory.StoreByte(Stack.Pop(), (byte)Stack.Pop());
        }

        private static void wFetch()
        {
            Stack[-1] = Memory.FetchShort(Stack[-1]);
        }

        private static void wStore()
        {
            Memory.StoreShort(Stack.Pop(), (short)Stack.Pop());
        }

		private static void memAdd()
        {
            var address = Stack.Pop();
            var delta = Stack.Pop();
            Memory.StoreInt(address, Memory.FetchInt(address) + delta);
        }

        private static void create(Tokenizer tokenizer)
        {
            var lcWord = tokenizer.NextToken().ToLower();
            Session.LastDefinedWord = lcWord;
            Vocabulary.CurrentVocabulary.AddDefinition(lcWord, new IntPrim(Memory.Here(), lcWord));
        }

        private static void strhead()
        {
            Stack.Push(Session.StringLengthSize);
        }

        private static void count()
        {
            var address = Stack.Pop();

            Stack.Push(address + StringLengthSize);

#pragma warning disable 162
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            // ReSharper disable once UnreachableCode
            Stack.Push(StringLengthSize == 1 ? Memory.FetchByte(address) : Memory.FetchInt(address));
#pragma warning restore 162
        }
		#endregion

		#region Logic operations
		private static void and()
        {
            var n2 = Stack.Pop();
            Stack[-1] = Stack[-1] & n2;
        }

        private static void or()
        {
            var n2 = Stack.Pop();
            Stack[-1] = Stack[-1] | n2;
        }

        private static void xor()
        {
            var n2 = Stack.Pop();
            Stack[-1] = Stack[-1] ^ n2;
        }

        private static void invert()
		{
            Stack[-1] = ~Stack[-1];
        }

        private static void lshift()
        {
            var u = Stack.Pop();
            Stack[-1] = Stack[-1] << u;
        }

        private static void rshift()
        {
            var u = Stack.Pop();
            Stack[-1] = Stack[-1] >> u;
        }
		#endregion

		#region Comparisons
		private static void eq()
        {
            var n2 = Stack.Pop();
            Stack[-1] = Stack[-1] == n2 ? -1 : 0;
        }

        private static void neq()
        {
            var n2 = Stack.Pop();
            Stack[-1] = Stack[-1] != n2 ? -1 : 0;
        }

        private static void leq()
        {
            var n2 = Stack.Pop();
            Stack[-1] = Stack[-1] <= n2 ? -1 : 0;
        }

		private static void geq()
        {
            var n2 = Stack.Pop();
            Stack[-1] = Stack[-1] >= n2 ? -1 : 0;
        }

        private static void zeq()
        {
            Stack[-1] = Stack[-1] == 0 ? -1 : 0;
        }

		private static void zneq()
        {
            Stack[-1] = Stack[-1] != 0 ? -1 : 0;
        }

        private static void zgt()
        {
            Stack[-1] = Stack[-1] > 0 ? -1 : 0;
        }

		private static void zlt()
        {
            Stack[-1] = Stack[-1] < 0 ? -1 : 0;
        }

        private static void lt()
        {
            var n2 = Stack.Pop();
            Stack[-1] = Stack[-1] < n2 ? -1 : 0;
        }

        private static void gt()
        {
            var n2 = Stack.Pop();
            Stack[-1] = Stack[-1] > n2 ? -1 : 0;
        }

        private static void ult()
        {
            var n2 = (uint)Stack.Pop();
            Stack[-1] = (uint)Stack[-1] < n2 ? -1 : 0;
        }

        private static void ugt()
        {
            var n2 = (uint)Stack.Pop();
            Stack[-1] = (uint)Stack[-1] > n2 ? -1 : 0;
        }
		#endregion

		#region Comments
		private static void lineComment(Tokenizer tokenizer)
        {
            while (tokenizer.NextToken(true, true) != "\n") ;
        }

        private static void mlComment(Tokenizer tokenizer)
        {
            while (tokenizer.NextToken() != ")") ;
        }
		#endregion

		#region Compiling
        private static void define(Tokenizer tokenizer)
        {
            DefinitionAction.Definition(tokenizer);
        }
		#endregion

		#region Stack manipulation
		private static void over2()
        {
            Stack.Push(Stack[-4]);
            Stack.Push(Stack[-4]);
        }

		private static void swap2()
        {
            (Stack[-4], Stack[-3], Stack[-2], Stack[-1]) =
                (Stack[-2], Stack[-1], Stack[-4], Stack[-3]);
        }

        private static void drop2()
        {
            Stack.Pop();
            Stack.Pop();
        }

        private static void dup2()
        {
            Stack.Data.Add(Stack[-2]);
            Stack.Data.Add(Stack[-2]);
        }

		private static void pick()
        {
            var index = -(Stack.Pop() + 1);
            Stack.Push(Stack[index]);
        }

        private static void rot()
        {
            (Stack[-3], Stack[-2], Stack[-1]) = (Stack[-2], Stack[-1], Stack[-3]);
        }

        private static void minusRot()
        {
            (Stack[-3], Stack[-2], Stack[-1]) = (Stack[-1], Stack[-3], Stack[-2]);
        }

		private static void tuck()
        {
            swap();
            over();
        }
        
        private static void nip()
        {
            Stack.Data.RemoveAt(Stack.Data.Count - 2);
        }

		private static void over()
        {
            Stack.Data.Add(Stack[-2]);
        }

		private static void swap()
        {
            (Stack[-1], Stack[-2]) = (Stack[-2], Stack[-1]);
        }

        private static void drop()
        {
            Stack.Pop();
        }

        private static void dup()
        {
            Stack.Data.Add(Stack[-1]);
        }

		private static void qDup()
		{
            if (Stack.Peek() != 0)
            {
                dup();
            }
        }
		#endregion

		#region Arithmetic
        private static void oneMinus()
        {
            Stack[-1]--;
        }

        private static void onePlus()
        {
            Stack[-1]++;
        }

        private static void min()
        {
            var v = Stack.Pop();
            Stack[-1] = Math.Min(v, Stack[-1]);
        }

		private static void max()
        {
            var v = Stack.Pop();
            Stack[-1] = Math.Max(v, Stack[-1]);
        }

		private static void abs()
        {
            Stack[-1] = Math.Abs(Stack[-1]);
        }

		private static void plus()
        {
            var a1 = Stack.Pop();
            Stack[-1] += a1;
        }

        private static void minus()
        {
            var a1 = Stack.Pop();
            Stack[-1] -= a1;
        }

        private static void times()
        {
            var a1 = Stack.Pop();
            Stack[-1] *= a1;
        }

        private static void divide()
        {
            var a1 = Stack.Pop();
            Stack[-1] /= a1;
        }

        private static void mod()
        {
            var a1 = Stack.Pop();
            Stack[-1] = Stack[-1] % a1;
        }

        private static void umStar()
        {
            var v = (ulong) Stack[-1] * (ulong) Stack[-2];
            Stack[-2] = (int)v;
            Stack[-1] = (int)(v >> 16);
        }

        private static void timesDiv()
        {
            var v = Stack.Pop() * (long)Stack.Pop();
            Stack[-1] = (int)(v / Stack[-1]);
        }

        private static void umMod()
        {
            var denominator = Stack.Pop();
            var numerator = Stack.Pop() + ((long) Stack.Pop()) << 16;
            Stack.Push((int)(numerator % denominator));
            Stack.Push((int)(numerator / denominator));
        }

        private static void negate()
        {
            Stack[-1] = -Stack[-1];
        }
		#endregion

		#region I/O
		private static void dot()
        {
            Console.Write(Stack.Pop());
        }

        private static void dotS()
        {
            Console.Write($"<{Stack.Count}>");
            foreach (var val in Stack)
            {
                Console.Write($" {val}");
            }
        }

        private static void emit()
        {
            Console.Write((char)Stack.Pop());
        }

        private static void cr()
        {
            Console.Write(Environment.NewLine);
        }

        static readonly string StringPage = new string(new[] {(char)12});
        private static void page()
        {
            Console.Write(StringPage);
        }

        private static void included()
        {
            var file = Memory.FetchSString();
            var stream = File.OpenText(file);
            if (stream == null)
            {
                throw new NfException($"Couldn't open {file}");
            }
            (new Interpreter()).Interpret(stream, false);
            stream.Close();
        }

        private static void type()
        {
			Console.Write(Memory.FetchSString());
        }

        static string _input = "abcdefghijklmnopqrstuvwxyz\n";
        private static int _ichInput = -1;

        private static void key()
        {
            Stack.Push(ReadKey());
        }

        private static char ReadKey()
        {
            if (Session.IsTesting)
            {
                _ichInput = (_ichInput + 1) % _input.Length;
                return _input[_ichInput];
            }
            else
            {
                return Console.ReadKey(true).KeyChar;
            }
        }
		#endregion
	}
}
