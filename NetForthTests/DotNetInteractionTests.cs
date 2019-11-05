using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetForth;
using FluentAssertions;
using static NetForth.Session;


namespace NetForthTests
{
	[TestClass]
	public class DotNetInteractionTests
    {
        private Session _session;
        private Interpreter _interpreter;

        [TestInitialize]
        public void TestInit()
        {
            _session = new Session();
            _interpreter = new Interpreter();

            // Define types we use below
            var script = @"
t"" System.DateTime"" constant tDatetime
t"" System.Int32[]"" constant tIntArray
t"" NetForthTests.DotNetInteractionTests, NetForthTests, Version = 1.0.0.0, Culture = neutral, PublicKeyToken = null"" constant tTests
t"" System.OutOfMemoryException"" constant tMemoryException";
			_interpreter.Interpret(script);
        }


		[TestCleanup]
        public void TestCleanup()
        {
            _session.Dispose();
        }

        static DateTime Now()
        {
            return DateTime.Now;
        }

        static int Month(DateTime dt)
        {
            return dt.Month;
        }

        [TestMethod]
        public void TestNString()
        {
            TestScript("n\" Darrell\" prop Length", 7);
        }

        [TestMethod]
        public void TestConstruction()
        {
            AddDotNetFn("Now", (Func<DateTime>)Now);
            AddDotNetFn("Month", (Func<DateTime, int>)Month);
            TestScript("Now Month", DateTime.Now.Month);
        }

        [TestMethod]
        public void TestProp()
        {
            TestScript("tDatetime sprop Now dup value sNow prop Day", DateTime.Now.Day);
            TestScript("sNow @ prop Day", DateTime.Now.Day);
            TestThrow("sNow @ prop Da5y", "Non-existent property in prop: Da5y");
        }

        // ReSharper disable once UnusedMember.Global
        public static int Square(int x)
        {
            return x * x;
        }

        [TestMethod]
        public void TestCall()
        {
            _interpreter.Interpret("tInt 1 tTests defStat Square fSquare");
			TestScript("10 fSquare", 100);

            // Don't run this a few milliseconds before midnight on the last day of the month!
            _interpreter.Interpret("tInt 1 tDatetime defMeth AddMonths am");
			TestScript("tDatetime sprop Now 1 swap am prop Month", DateTime.Now.Month + 1);
        }

        [TestMethod]
        public void TestConstructor()
        {
            _interpreter.Interpret("tString 1 tMemoryException defCnst mExcpt");
            TestScript("n\" Darrell\" mExcpt prop Message prop Length", 7);
        }

        static int[] ArrayMaker(int count)
        {
            return Enumerable.Range(0, count).ToArray();
        }

        [TestMethod]
        public void TestIndexer()
        {
            _interpreter.Interpret("tInt 1 tString defIndx strIndexer");
            _interpreter.Interpret("tInt 1 tIntArray defIndx arrIndexer");
            AddDotNetFn("arrayMaker", (Func<int, int[]>)ArrayMaker);
            TestScript("1 n\" Darrell\" strIndexer", 'a');
            TestScript("10 arrayMaker 9 swap arrIndexer", 9);
        }

		private void TestThrow(string script, string msg = null)
        {
            Action act = () => _interpreter.Interpret(script);
            if (msg == null)
            {
                act.Should().Throw<NfException>();
            }
			else
            {
                act
                    .Should().Throw<NfException>()
                    .WithMessage(msg);
            }

		}

		private void TestScript(string script, int expected)
        {
            try
            {
                _interpreter.Interpret(script);
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
