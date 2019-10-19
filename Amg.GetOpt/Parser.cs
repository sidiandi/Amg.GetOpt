using System.Collections.Generic;
using System.Linq;
using Amg.GetOpt.Tokens;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<Amg.GetOpt.Tokens.Token>;

namespace Amg.GetOpt
{
    static internal class Parser
    {
        static Opt<T> WithOptions<T>(T value, IEnumerable<Option> options)
        {
            return new Opt<T>(value, options);
        }

        static Opt<T> WithOptions<T, Y>(T value, IEnumerable<Opt<Y>> options)
        {
            return new Opt<T>(value, options.SelectMany(_ => _.Options));
        }

        internal class Opt<T>
        { 
            public Opt(T value, IEnumerable<Option> options)
            {
                Value = value;
                Options = options;
            }

            public IEnumerable<Option> Options { get; }
            public T Value;

            public override string ToString() => Value!.ToString();
        }

        internal class Command
        { 
            public Command(Opt<Arg> name, IEnumerable<Opt<Arg>> args)
            {
                Name = name;
                Args = args;
            }

            public Opt<Arg> Name { get; }
            public IEnumerable<Opt<Arg>> Args { get; }
        }

        internal class Option
        { }

        
        internal class LongOption: Option
        {
            public LongOption(Amg.GetOpt.Tokens.LongOption name, IEnumerable<Arg> args)
            {
                Name = name;
                Args = args;
            }

            public Amg.GetOpt.Tokens.LongOption Name { get; }
            public IEnumerable<Arg> Args { get; }
        }

        internal class ShortOption : Option
        {
            public ShortOption(Amg.GetOpt.Tokens.ShortOption name, IEnumerable<Arg> args)
            {
                Name = name;
                Args = args;
            }

            public Amg.GetOpt.Tokens.ShortOption Name { get; }
            public IEnumerable<Arg> Args { get; }
        }


        internal class CommandLine
        {
            public CommandLine(IEnumerable<Option> options, IEnumerable<Command> commands)
            {
                Options = options;
                Commands = commands;
            }

            public IEnumerable<Option> Options { get; }
            public IEnumerable<Command> Commands { get; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S1481:Unused local variables should be removed", Justification = "<Pending>")]
        public static CommandLine Parse(string[] commandLineArguments, object instance)
        {
            var tokens = Amg.GetOpt.Tokens.Token.Tokenize(commandLineArguments);
            var commandProvider = new CommandProvider(instance);

            var arg = Any.OfType<Arg>();
            Parser<Token, Opt<Arg>>? optionArg = null;

            var longOption = Any.OfType<Amg.GetOpt.Tokens.LongOption>().Bind(_ => LongOptionParser(_, commandProvider));
            var shortOption = Any.OfType<Amg.GetOpt.Tokens.ShortOption>().Bind(_ => ShortOptionParser(_, commandProvider));
            var option = OneOf(Try(longOption), Try(shortOption));
            var options = Try(option).Many();
            optionArg = Map((arg, options) => WithOptions(arg, options), arg, options);
            var optionArgs = Try(optionArg).Many();
            var commandName = optionArg;

            Parser<Token, Option> LongOptionParser(Amg.GetOpt.Tokens.LongOption optionName, CommandProvider commandProvider)
            {
                var option = commandProvider.LongGetOption(optionName.Name);
                return arg.Repeat(0).Map(values => (Option) new LongOption(optionName, values));
            }

            Parser<Token, Option> ShortOptionParser(Amg.GetOpt.Tokens.ShortOption optionName, CommandProvider commandProvider)
            {
                var option = commandProvider.ShortGetOption(optionName.Name);
                return arg.Repeat(0).Map(values => (Option) new ShortOption(optionName, values));
            }

            Parser<Token, Command> CommandParser(Opt<Arg> commandName, CommandProvider commandProvider)
            {
                var command = commandProvider.GetCommand(commandName.Value.Value);
                var parameters = command.Method.GetParameters();
                return optionArg.Repeat(parameters.Length).Map(values => new Command(commandName, values));
            }

            var command = commandName.Bind(_ => CommandParser(_, commandProvider));
            var commands = Try(command).Many();
            var commandLine = Map((o,c) => new CommandLine(o, c), options, commands);

            var ast = commandLine.Parse(tokens);
            return ast.Value;
        }
    }
}
