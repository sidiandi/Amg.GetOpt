using NUnit.Framework;
using Pidgin;
using System;
using System.Linq;
using static Pidgin.Parser;
using static Pidgin.Parser<Amg.GetOpt.Tokens.Token>;

namespace Amg.GetOpt.Test
{
    public class Tests
    {
        [Test]
        public void TryPidgin()
        {
            var tokens = Amg.GetOpt.Tokens.Token.Tokenize(new[] { "hello", "--long-option", "-h", "--long-option=value", "-vdetailed", "--", "--not-an-option" });
            Any.Many().Parse(tokens);
            Assert.That(true);
        }

        [Test]
        public void Parse()
        {
            var args = new[] { "--long-option", "-h", "--long-option=value", "hello", "-vdetailed", "--", "--not-an-option" };
            var o = new TestCommandObject();
            Parser.Parse(args, o);
            Assert.That(true);
        }

        [Test]
        public void Parse2()
        {
            var args = new[] { "add", "1", "--name", "2", "subtract", "3", "2" };
            var o = new TestCommandObject();
            var ast = Parser.Parse(args, o);
            Console.WriteLine(ast);
            Assert.That(ast.Commands.Count() == 2);
            Assert.That(ast.Options.Count() == 1);
        }
    }
}