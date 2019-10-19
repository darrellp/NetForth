using System;
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

        [TestInitialize]
        public void TestInit()
        {
            _session = new Session();
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
            TestScript("t\" System.DateTime\" sprop Now dup value sNow prop Day", DateTime.Now.Day);
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
            AddDotNetFn("Now", (Func<DateTime>)Now);
            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
            TestScript("Now call ToString noprms prop Length", DateTime.Now.ToString().Length);
            TestScript("10 t\" NetForthTests.DotNetInteractionTests, NetForthTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\" scall Square i", 100);
        }

		private static void TestThrow(string script, string msg = null)
        {
            var intrp = new Interpreter();
            Action act = () => intrp.Interpret(script);
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

		private static void TestScript(string script, int expected)
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
