﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetForth;
using FluentAssertions;
using static NetForth.Session;

namespace NetForthTests
{
	[TestClass]
	public class RootFunctionTests
	{
        [TestMethod]
        public void TestSub()
        {
            TestScript("20 10 -", 10);
        }

        [TestMethod]
		public void TestSwap()
		{
			TestScript("20 10 swap -", -10);
		}

		[TestMethod]
		public void TestConstant()
		{
			TestScript("20 constant n n", 20);
		}

		[TestMethod]
		public void TestIf()
		{
			TestScript(": iftest if 100 else 200 then ; 0 iftest 1 iftest +", 300);
		}

        [TestMethod]
        public void TestTick()
        {
            TestScript("' dup", 0);
            TestScript(": 1+ 1 + ; 10 ' 1+ value del del @ execute", 11);
        }

        [TestMethod]
        public void TestBegin()
        {
			TestScript(": dotest 0 begin 1 + dup 5 > if exit then again ; dotest", 6);
			TestScript(": dotest 0 begin 1 + dup 5 > until ; dotest", 6);
			TestScript(": dotest 0 0 begin 1 + dup 6 < while swap 2 + swap repeat drop ; dotest", 10);
        }

		[TestMethod]
		public void TestDo()
		{
			TestScript(": dotest 0 10 1 do over + loop nip ; 5 dotest", 45);
		}

		[TestMethod]
		public void TestI()
		{
			TestScript(": dotest 0 10 0 do i + loop ; dotest", 45);
            TestScript(": dotest 0 2 0 do 10 0 do i + loop loop ; dotest", 90);
            TestScript(": dotest 0 2 0 do 10 0 do j + loop loop ; dotest", 10);
		}

		[TestMethod]
		public void TestLeave()
		{
            TestScript(": dotest 0 10 1 do i + i 5 > if leave then 100 + loop ; dotest", 521);
            TestScript(": dotest 0 10 1 do i + i 5 > ?leave 100 + loop ; dotest", 521);
		}


        [TestMethod]
        public void TestExit()
        {
			// Exit should leave entire definition which should leave 521 on the stack
            TestScript(": dotest 0 10 1 do i + i 5 > if exit then 100 + loop drop 1000 ; dotest", 521);
			// Leave should leave just the do loop and evealuate the rest of the definition putting 1000 on the stack
            TestScript(": dotest 0 10 1 do i + i 5 > if leave then 100 + loop drop 1000 ; dotest", 1000);
			// Exit should leave only the current dfn.  The caller should still put 500 on the stack
            TestScript(": dotest 0 10 1 do i + i 5 > if exit then 100 + loop drop 1000 ; : dodotest dotest drop 500 ; dodotest", 500);
        }

		[TestMethod]
		public void TestIncLoop()
		{
			TestScript(": dotest 0 -10 0 do i + -1 +loop ; dotest", -45);
		}

		[TestMethod]
		public void TestCountedString()
        {
            var stringLengthRetriever = Session.StringLengthSize == 1 ? "b@" : "@";
            var lengthScript = $": dotest C\" Hello World!\" ; dotest {stringLengthRetriever}";

            // Length
            TestScript(lengthScript, 12);
            // First char ("H")
            TestScript(": dotest C\" Hello World!\" ; dotest strhead + c@", 72);
            // Last char ("!")
            TestScript(": dotest C\" Hello World!\" ; dotest strhead 11 chars + + c@", 33);
		}

        [TestMethod]
        public void TestStackString()
        {
            TestScript($": dotest S\" Hello World!\" ; dotest swap 1 chars + c@ +", 12 + 'e');
        }

        [TestMethod]
        public void TestCharWord()
        {
            TestScript(": dotest [char] a ; dotest", 'a');
            TestScript("char a", 'a');
        }

        [TestMethod]
        public void TestCreate()
        {
            using (var fsession = new Session())
            {
				var intrp = new Interpreter();
				intrp.Interpret("create thisSpot thisSpot");
				Stack[0].Should().Be((int)Session.ForthMemory);
                intrp.Interpret(": dotest create ; dotest doggy doggy");
                Stack[0].Should().Be((int)Session.ForthMemory);
            }

		}

		[TestMethod]
        public void TestComma()
        {
            using (var unused = new Session())
            {
                var intrp = new Interpreter();
                intrp.Interpret("5 ,");
                var val = Memory.FetchInt(Memory.Here() - sizeof(int));
                val.Should().Be(5);
                intrp.Interpret("char P c,");
                var valc = Memory.FetchChar(Memory.Here() - sizeof(char));
                valc.Should().Be('P');
            }
        }

        [TestMethod]
        public void TestAllocation()
        {
            using (var unused = new Session())
            {
                var intrp = new Interpreter();
                intrp.Interpret("here");
                Stack[0].Should().Be(Memory.Here());
                var before = Memory.Here();
                intrp.Interpret("10 allot");
                Memory.Here().Should().Be(before + 10);
            }
        }

		[TestMethod]
        public void TestDoes()
        {
            TestScript(": myCnst create , does> @ ; 10 myCnst ten ten", 10);
        }

		[TestMethod]
        public void TestTemplate()
        {
        }

        private void TestScript(string script, int expected)
        {
            using (var unused = new Session())
            {
                var intrp = new Interpreter();
                try
                {
                    intrp.Interpret(script);
                    Stack.Should().HaveCount(1);
                    Stack[0].Should().Be(expected);
                }
                finally
                {
                    Stack.Clear();
                }
            }
        }
	}
}
