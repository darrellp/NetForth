using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetForth;
using FluentAssertions;
using static NetForth.Session;

namespace NetForthTests
{
    [TestClass]
    public class InterpreterTests
    {
        [TestMethod]
        public void TestInterpret()
        {
            NewTestScript("20 10 dup + +", 40);
        }

        [TestMethod]
        public void TestComments()
        {
            using (var nf = new Session())
            {
                var code = @"
20 10 \ This is a test
dup ( This is
      a multiline comment ) +
+";
                NewTestScript(code, 40);
            }
        }

        [TestMethod]
        public void TestDefine()
        {
            NewTestScript(": square ( n -- n^2 ) dup * ; 3 square", 9);
        }

        private void NewTestScript(string script, int expected)
        {
            using (var nf = new Session())
            {
                var intrp = new Interpreter(script);
                intrp.Interpret();
                Stack.Should().HaveCount(1);
                Stack[0].Should().Be(expected);
                Stack.Clear();
            }
        }

        [TestMethod]
        public void TestUndefinedWord()
        {
            using (var nf = new Session())
            {
                var intrp = new Interpreter("doggy");
                Action act = () => intrp.Interpret();
                act
                    .Should().Throw<NfException>()
                    .WithMessage("Couldn't locate word doggy");
            }
        }

        [TestMethod]
        public void TestInvalidSession()
        {
            Action act = () => new Interpreter("10");
            act
                .Should().Throw<NfException>()
                .WithMessage("Interpreting without a valid session");
        }

        [TestMethod]
        public void TestStackUnderflow()
        {
            using (var nf = new Session())
            {
                var intrp = new Interpreter("*");
                Action act = () => intrp.Interpret();
                act
                    .Should().Throw<NfException>()
                    .WithMessage("Stack underflow");
            }
        }

        [TestMethod]
        public void TestStackIndexRange()
        {
            using (var nf = new Session())
            {
                Action act = () =>
                {
                    // ReSharper disable once UnusedVariable
                    var i = Stack[-1];
                };
                act
                    .Should().Throw<NfException>()
                    .WithMessage("Index out of range");

                act = () => { Stack[-1] = 0; };
                act
                    .Should().Throw<NfException>()
                    .WithMessage("Index out of range");
            }
        }
	}
}
