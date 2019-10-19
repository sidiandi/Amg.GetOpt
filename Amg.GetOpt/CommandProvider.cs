using Amg.Extensions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Amg.GetOpt
{

    class CommandProvider : ICommandProvider
    {
        private readonly object commandObject;

        public CommandProvider(object commandObject)
        {
            this.commandObject = commandObject;
        }

        public ICommand GetCommand(string name)
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

        public IOption GetLongOption(string optionName)
        {
            return Options().FindByName(_ => _.Long, optionName, "options");
        }

        public IOption GetShortOption(string optionName)
        {
            return Options()
                .Where(_ => _.Short != null)
                .FindByName(_ => _.Short, optionName, "options");
        }
    }
}

