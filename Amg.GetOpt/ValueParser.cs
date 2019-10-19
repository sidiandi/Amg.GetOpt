using Amg.Extensions;
using System;

namespace Amg.GetOpt
{
    internal class ValueParser : IValueParser
    {
        public bool TryParse(ParserState args, Type toType, out object? value)
        {
            string Consume()
            {
                return args.Consume();
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
                    var enumName = Enum.GetNames(toType).FindByName(_ => _, Consume(), "values");

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
                throw new ArgumentException($"{args.Current.Quote()} is not a value of type {toType.Name}.", e);
            }
        }
    }
}