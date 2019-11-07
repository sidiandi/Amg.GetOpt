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
            var o = new WithStandardOptions(new TestCommandObject());
            var p = new CommandProviderImplementation(o);
            var helpMessage = TextFormatExtensions.GetWritable(_ => Amg.GetOpt.Help.PrintHelpMessage(_, p)).ToString();
            Console.WriteLine(helpMessage);
            Assert.That(helpMessage, Does.Contain("Run a command."));
            Assert.That(helpMessage, Does.Contain("Options:"));
            Assert.Pass(helpMessage);
        }

        [Test]
        public void HelpNoCommands()
        {
            var o = new OnlyDefaultCommand();
            var p = new CommandProviderImplementation(o);
            var helpMessage = TextFormatExtensions.GetWritable(_ => Amg.GetOpt.Help.PrintHelpMessage(_, p)).ToString();
            Assert.Pass(helpMessage);
        }

        [Test]
        public void DefaultCommand()
        {
            var o = new WithDefaultCommand();
            var exitCode = GetOpt.Run(new string[] { "1", "1" }, o);
            Assert.AreEqual(ExitCode.Success, exitCode);
            Assert.That(o.result, Is.EqualTo(2));
        }

        [Test]
        public void HelpForDefaultCommand()
        {
            var o = new WithDefaultCommand();
            var (output, error) = CaptureOutput(() =>
             {
                 var exitCode = GetOpt.Run(new string[] { "-h" }, o);
                 Assert.AreEqual(ExitCode.HelpDisplayed, exitCode);
             });
            Console.WriteLine(output);
            Assert.That(!output.Contains("do-something"));
        }

        [Test]
        public void HelpForDefaultCommandNoParameters()
        {
            var o = new WithDefaultCommandNoParameters();
            var (output, error) = CaptureOutput(() =>
            {
                var exitCode = GetOpt.Run(new string[] { "-h" }, o);
                Assert.AreEqual(ExitCode.HelpDisplayed, exitCode);
            });
            Console.WriteLine(output);
            Assert.That(!output.Contains("do-something"));
        }

        [Test]
        public void DefaultCommandWithoutParameters()
        {
            var o = new WithDefaultCommandNoParameters();
            int exitCode = 0;
            var (output, error) = CaptureOutput(() =>
            {
                exitCode = GetOpt.Run(new string[] { "1", "1" }, o);
            });
            Console.WriteLine(error);
            Assert.AreEqual(ExitCode.CommandLineError, exitCode);
        }
    }
}
