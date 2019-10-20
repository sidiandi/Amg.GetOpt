using Amg.Extensions;
using System;

namespace Amg.GetOpt
{
    internal class ValueParser : IValueParser
    {
        public bool TryParse(ParserState args, Type toType, out object? value)
        {
            var temp = args.Clone();
            string Consume()
            {
                if (!temp.HasCurrent)
                {
                    throw new CommandLineException(args, "Value is missing.");
                }
                return temp.Consume();
            }

            try
            {
                if (toType == typeof(bool))
                {
                    var success = bool.TryParse(Consume(), out var typedValue);
                    value = typedValue;
                    return success;
                }
                else if (toType == typeof(int))
                {
                    var success = int.TryParse(Consume(), out var typedValue);
                    value = typedValue;
                    return success;
                }
                else if (toType == typeof(double))
                {
                    var success = double.TryParse(Consume(), out var typedValue);
                    value = typedValue;
                    return success;
                }
                else if (toType == typeof(string))
                {
                    value = Consume();
                    return true;
                }
                else if (toType.IsEnum)
                {
                    var enumName = Enum.GetNames(toType).FindByName(_ => Parser.LongNameForCsharpIdentifier(_), Consume(), "values");

                    if (enumName == null)
                    {
                        value = null;
                        return false;
                    }
                    else
                    {
                        value = Enum.Parse(toType, enumName);
                        return true;
                    }
                }
                throw new ArgumentException($"Cannot parse {args.Current.Quote()} as {toType.Name}.");
            }
            catch (Exception e)
            {
                if (e is CommandLineException)
                {
                    throw;
                }
                throw new CommandLineException(args, $"{args.Current.Quote()} is not a value of type {toType.Name}.", e);
            }
            finally
            {
                args.SetPos(temp);
            }
        }
    }
}