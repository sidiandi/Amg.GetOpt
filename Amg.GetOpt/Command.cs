using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Amg.GetOpt
{
    internal class Command : ICommand
    {
        private readonly object instance;
        public MethodInfo Method { get; private set; }

        public Command(object instance, MethodInfo method)
        {
            this.instance = instance;
            this.Method = method;
            this.Name = Parser.LongName(method);
        }

        public Task<object?> Invoke(ParserState args, IValueParser valueParser)
        {
            object? Get(ParameterInfo parameter)
            {
                if (!args.HasCurrent)
                {
                    if (parameter.HasDefaultValue)
                    {
                        return parameter.DefaultValue;
                    }
                    else
                    {
                        throw new CommandLineException(args, $"missing parameter {parameter.Name} for command {this.Name}.");
                    }
                }

                var temp = args.Clone();
                if (valueParser.TryParse(temp, parameter.ParameterType, out var value))
                {
                    args.SetPos(temp);
                    return value;
                }
                else
                {
                    throw new CommandLineException(args, $"Argument {parameter.Name} of command {this.Name} has value {args.Current}. It cannot be interpreted as type {parameter.ParameterType}.");
                }
            }

            var parameters = Method.GetParameters().Select(Get).ToArray();
            return AsTask(Method.Invoke(instance, parameters));
        }

        /// <summary>
        /// Waits if returnValue is Task
        /// </summary>
        /// <param name="returnValue"></param>
        /// <returns></returns>
        static Task<object?> AsTask(object returnValue)
        {
            if (returnValue is Task task)
            {
                var type = returnValue.GetType();
                var resultProperty = type.GetProperty("Result");
                return task.ContinueWith((_) =>
                {
                    try
                    {
                        var result = resultProperty == null
                            ? null
                            : resultProperty.GetValue(task);
                        return result;
                    }
                    catch (TargetInvocationException targetInvocationException)
                    {
                        throw targetInvocationException.InnerException.InnerException;
                    }
                });
            }
            else
            {
                return Task.FromResult<object?>(returnValue);
            }
        }

        public string Name { get; }
    }

}

