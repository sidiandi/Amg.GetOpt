using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Amg.GetOpt.Test")]

namespace Amg.GetOpt
{
    public static class ExitCode
    {
        public const int Success = 0;
        public const int UnknownError = 1;
        public const int HelpDisplayed = 3;
        public const int CommandLineError = 4;
        public const int CommandFailed = 5;
    }

    public static class GetOpt
    {
        public static int Run(string[] args, object commandObject)
        {
            var commandProvider = new CommandProvider(commandObject);
            var parser = new Parser(commandProvider);
            parser.Parse(args);
            try
            {
                var result = parser.Run().Result;
                foreach (var i in result)
                {
                    Console.WriteLine(i);
                }
            }
            catch (AggregateException aex)
            {
                int exitCode = ExitCode.Success;

                aex.Handle(exception =>
                {
                    if (exception is NoDefaultCommandException)
                    {
                        Help.PrintHelpMessage(Console.Out, commandProvider);
                        exitCode = ExitCode.HelpDisplayed;
                        return true;
                    }
                    else
                    {
                        Console.Error.WriteLine(exception);
                        exitCode = ExitCode.UnknownError;
                        return true;
                    }
                });

                return exitCode;
            }

            return ExitCode.Success;
        }
    }
}
