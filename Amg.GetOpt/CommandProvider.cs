using Amg.Extensions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Amg.GetOpt
{

    class CommandProvider
    {
        private readonly object commandObject;

        public CommandProvider(object commandObject)
        {
            this.commandObject = commandObject;
        }

        public Command GetCommand(string name)
        {
            return Commands().FindByName(_ => _.Name, name, "commands");
        }

        IEnumerable<Command> Commands()
        {
            return commandObject.GetType().GetMethods()
                .Where(IsCommand)
                .Select(_ => new Command(commandObject, _));
        }

        static bool IsCommand(MethodInfo m)
        {
            return m.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>() != null;
        }

        IEnumerable<Option> Options()
        {
            return commandObject.GetType().GetProperties()
                .Where(IsOption)
                .Select(_ => new Option(commandObject, _));
        }

        static bool IsOption(PropertyInfo p)
        {
            return p.GetCustomAttribute<DescriptionAttribute>() != null;
        }

        internal Option LongGetOption(string optionName)
        {
            return Options().FindByName(_ => _.Long, optionName, "options");
        }

        internal Option ShortGetOption(string optionName)
        {
            return Options()
                .FindByName(_ => _.Short, optionName, "options");
        }
    }

    internal class Command
    {
        private readonly object instance;
        public MethodInfo Method { get; private set; }

        public Command(object instance, MethodInfo method)
        {
            this.instance = instance;
            this.Method = method;
        }

        public object Invoke(object?[] parameters)
        {
            return Method.Invoke(instance, parameters);
        }

        public string Name => Method.Name;
    }

    internal class CommandInvoke
    {
        public CommandInvoke(Command command, object?[] parameters)
        {
            Command = command;
            Parameters = parameters;
        }

        public Command Command { get; }
        public object?[] Parameters { get; }

        public object Invoke() => Command.Invoke(Parameters);
    }

    internal class OptionSet
    {
        public OptionSet(Option option, object? value)
        {
            Option = option;
            Value = value;
        }

        public void SetValue() => Option.SetValue(Value);

        public Option Option { get; }
        public object? Value { get; }
    }

}

