using System.Reflection;

namespace Amg.GetOpt.Extensions
{
    internal static class Extensions
    {
        public static bool Has<T>(this ICustomAttributeProvider attributeProvider)
            => attributeProvider.IsDefined(typeof(T), true);
    }
}
