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
        private Session session;

        [TestInitialize]
        public void TestInit()
        {
            session = new Session();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            session.Dispose();
        }

		[TestMethod]
		public void TestCall()
		{
            int Add(int a, int b)
            {
                return a + b;
            }

            Stack.Push(10);
            AddDotNetFn("Add", (Func<int, int, int>)Add);
            TestScript("10 20 Add", 30);
		}

        [TestMethod]
        public void TestConstruction()
        {
            DateTime Now()
            {
                return DateTime.Now;
            }

            int Month(DateTime dt)
            {
                return dt.Month;
            }

            AddDotNetFn("Now", (Func<DateTime>)Now);
            AddDotNetFn("Month", (Func<DateTime, int>)Month);
            TestScript("Now Month", DateTime.Now.Month);
        }

		private void TestScript(string script, int expected)
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
