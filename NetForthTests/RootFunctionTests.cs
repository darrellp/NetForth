using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetForth;
using FluentAssertions;
using static NetForth.DataStack;

namespace NetForthTests
{
	[TestClass]
	public class RootFunctionTests
	{
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
			TestScript(": iftest if 100 else 200 then ; 0 iftest", 100);
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
            TestScript(": dotest 0 10 1 do i + i 5 > if leave then 100 + loop drop 1000 ; dotest", 1000);
            TestScript(": dotest 0 10 1 do i + i 5 > if exit then 100 + loop drop 1000 ; dotest", 521);
        }
		[TestMethod]
		public void TestIncLoop()
		{
			TestScript(": dotest 0 -10 0 do i + -1 +loop ; dotest", -45);
		}

		[TestMethod]
		public void TestCountedString()
		{
            // Length
			TestScript(": dotest C\" Hello World!\" ; dotest @", 12);
            // First char ("H")
            TestScript(": dotest C\" Hello World!\" ; dotest 1 cells + c@", 72);
            // Last char ("!")
            TestScript(": dotest C\" Hello World!\" ; dotest 1 cells 11 chars + + c@", 33);
		}

		private void TestScript(string script, int expected)
        {
            using (var nf = new Session())
            {
                var intrp = new Interpreter(script);
				try
				{
					intrp.InterpretAll();
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
