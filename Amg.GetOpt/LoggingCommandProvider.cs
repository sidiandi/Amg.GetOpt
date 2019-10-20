using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;

namespace Amg.GetOpt
{
    public static class LoggingCommandProviderExtensions
    { 
        public static ICommandProvider Log(this ICommandProvider next, ILogger logger)
        {
            return new LoggingCommandProvider(next, logger);
        }
    }

    class LoggingCommandProvider : ICommandProvider
    {
        private readonly ICommandProvider next;
        private readonly ILogger logger;

        public LoggingCommandProvider(ICommandProvider next, ILogger logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public ICommand GetCommand(string name)
        {
            return new Command(next.GetCommand(name), logger);
        }

        class Command : ICommand
        {
            private readonly ICommand next;

            public Command(ICommand next, ILogger logger)
            {
                this.next = next;
                Logger = logger;
            }

            public ILogger Logger { get; }

            public string Name => next.Name;

            public string Syntax => next.Syntax;

            public string Description => next.Description;

            public bool IsDefault => next.IsDefault;

            public Task<object?> Invoke(ParserState args, IValueParser valueParser)
            {
                Logger.Information("Invoke {command}", Name);
                return next.Invoke(args, valueParser);
            }
        }

        public IOption GetLongOption(string optionName)
        {
            return new Option(next.GetLongOption(optionName), logger);
        }

        class Option: IOption
        {
            private readonly IOption next;
            private readonly ILogger logger;

            public Option(IOption next, ILogger logger)
            {
                this.next = next;
                this.logger = logger;
            }

            public string Long => next.Long;

            public string? Short => next.Short;

            public string Syntax => next.Syntax;

            public string Description => next.Description;

            public void Set(ref string? appendedValue, ParserState args, IValueParser valueParser)
            {
                logger.Information("Set {option}", Long);
                next.Set(ref appendedValue, args, valueParser);
            }
        }

        public IOption GetShortOption(string optionName)
        {
            return new Option(next.GetShortOption(optionName), logger);
        }

        public IEnumerable<IOption> Options() => next.Options();
        public IEnumerable<ICommand> Commands() => next.Commands();
    }
}
