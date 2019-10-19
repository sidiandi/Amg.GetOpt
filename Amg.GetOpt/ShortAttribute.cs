using System;
using System.Reflection;

namespace Amg.GetOpt
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ShortAttribute : Attribute
    {
        public string ShortName { get; }

        public ShortAttribute(string shortName)
        {
            this.ShortName = shortName;
        }

        public static string? Get(PropertyInfo p)
        {
            var a = p.GetCustomAttribute<ShortAttribute>();
            return a == null ? null : a.ShortName;
        }
    }
}