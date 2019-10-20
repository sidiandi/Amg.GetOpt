using Serilog;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Amg.GetOpt
{
    public static class CommandProviderFactory
    {
        public static ICommandProvider FromObject(object objectWithAttributes)
        {
            return new CommandProviderImplementation(objectWithAttributes);
        }

        public static ICommandProvider Log(this ICommandProvider next, ILogger logger)
        {
            return new LoggingCommandProvider(next, logger);
        }
    }

    internal class CommandProviderImplementation : ICommandProvider
    {
        private readonly object commandObject;

        public CommandProviderImplementation(object commandObject)
        {
            this.commandObject = commandObject;
        }

        IEnumerable<ICommandProvider> CommandProviders => commandObject.GetType().GetProperties()
            .Where(IsCommandProvider)
            .Select(_ => _.GetValue(commandObject))
            .Where(_ => _ != null)
            .Select(_ => new CommandProviderImplementation(_));

        static bool IsCommandProvider(PropertyInfo p)
            => p.GetCustomAttribute<CommandProviderAttribute>() != null;

        public IEnumerable<ICommand> Commands()
        {
            return commandObject.GetType().GetMethods()
                .Where(IsCommand)
                .Select(_ => new Command(commandObject, _))
                .Concat(CommandProviders.SelectMany(_ => _.Commands()));
        }

        static bool IsCommand(MethodInfo m)
        {
            return m.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>() != null;
        }

        public IEnumerable<IOption> Options()
        {
            return commandObject.GetType().GetProperties()
                .Where(IsOption)
                .Select(_ => new Option(commandObject, _))
                .Concat(CommandProviders.SelectMany(_ => _.Options()));
        }

        static bool IsOption(PropertyInfo p)
        {
            return p.GetCustomAttribute<DescriptionAttribute>() != null;
        }
    }
}

