using Amg.Extensions;
using NUnit.Framework;
using System;

namespace Amg.GetOpt.Test
{
    [TestFixture]
    class GetOptTests : TestBase
    {
        [Test]
        public void Run()
        {
            var co = new TestCommandObject();
            var exitCode = GetOpt.Run(new[]
            {
                "add",
                "1",
                "--name",
                "Alice",
                "2",
                "subtract",
                "3",
                "2",
                "greet"
            }, co);
            Assert.That(exitCode, Is.EqualTo(0));
            Assert.That(co.Name, Is.EqualTo("Alice"));
        }

        [Test]
        public void ArgumentMissing()
        {
            var co = new TestCommandObject();
            var exitCode = GetOpt.Run(new[]
            {
                "add",
                "1",
            }, co);
            Assert.That(exitCode, Is.Not.EqualTo(0));
        }

        [Test]
        public void Help()
        {
            var o = new TestCommandObject();
            var p = new CommandProvider(o);
            var helpMessage = TextFormatExtensions.GetWritable(_ => Amg.GetOpt.Help.PrintHelpMessage(_, p)).ToString();
            Assert.That(helpMessage, Does.Contain("Commands:"));
            Assert.That(helpMessage, Does.Contain("Options:"));
            Assert.Pass(helpMessage);
            Assert.That(helpMessage, Is.EqualTo(@"Usage: testhost [options] [command] [arguments]...

Commands:
add <a: int32> <b: int32>      add two numbers           
greet                          Say hello                 
subtract <a: int32> <b: int32> subtract two numbers      
takes-string <value>           Method that takes a string

Options:
-h|--help                             Show help            
--long-option <string>                Option with long name
--name <string>                       name to be greeted   
-v|--verbosity <quiet|error|detailed> Enum option          
"));
        }

        [Test]
        public void DefaultCommand()
        {
            var o = new WithDefaultCommand();
            var exitCode = GetOpt.Run(new string[] { }, o);
            Assert.AreEqual(ExitCode.Success, exitCode);
            Assert.That(o.done);
        }

        [Test]
        public void NoDefaultCommandPrintsHelp()
        {
            var o = new TestCommandObject();
            var (output, error) = CaptureOutput(() =>
            {
                var exitCode = GetOpt.Run(new string[] { }, o);
                Assert.That(exitCode, Is.EqualTo(ExitCode.HelpDisplayed));
            });
            Assert.That(output.Contains("Usage:"));
        }
    }
}
