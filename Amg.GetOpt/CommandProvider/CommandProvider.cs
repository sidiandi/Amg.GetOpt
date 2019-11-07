using Amg.Extensions;
using Serilog;
using System;
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

        IEnumerable<ICommandProvider>? commandProviders = null;

        IEnumerable<ICommandProvider> CommandProviders
        {
            get
            {
                if (commandProviders == null)
                {
                    commandProviders = commandObject.GetType().GetProperties()
                        .Where(IsCommandProvider)
                        .Select(_ => _.GetValue(commandObject))
                        .Where(_ => _ != null)
                        .Select(_ => new CommandProviderImplementation(_))
                        .ToList();
                }
                return commandProviders;
            }
        }

        static bool IsCommandProvider(PropertyInfo p)
            => p.GetCustomAttribute<CommandProviderAttribute>() != null;

        IEnumerable<ICommand>? commands = null;
        public IEnumerable<ICommand> Commands
        {
            get
            {
                if (commands == null)
                {
                    commands = commandObject.GetType().GetMethods()
                        .Where(IsCommand)
                        .Select(_ => new Command(commandObject, _))
                        .Concat(CommandProviders.SelectMany(_ => _.Commands))
                        .Distinct(_ => _.Name);
                }
                return commands;
            }
        }

        static bool IsCommand(MethodInfo m)
        {
            return m.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>() != null;
        }

        IEnumerable<IOption>? options = null;
        public IEnumerable<IOption> Options
        {
            get
            {
                if (options == null)
                {
                    options = commandObject.GetType().GetProperties()
                    .Where(IsOption)
                    .Select(_ => new Option(commandObject, _))
                    .Concat(CommandProviders.SelectMany(_ => _.Options))
                    .Distinct(_ => _.Long);
                }
                return options;
            }
        }

        public int? OnOptionsParsed(Parser parser)
        {
            if (commandObject is ICommandObject commandObjectInterface)
            {
                var exitCode = commandObjectInterface.OnOptionsParsed(parser);
                if (exitCode != null)
                {
                    return exitCode;
                }
            }

            return CommandProviders.Select(_ => _.OnOptionsParsed(parser))
                .FirstOrDefault(_ => _ != null);
        }

        static bool IsOption(PropertyInfo p)
        {
            return p.GetCustomAttribute<DescriptionAttribute>() != null;
        }
    }
}

