namespace EmailTamer.Core.Extensions;

public static class ObjectExtensions
{
    public static TResult Map<T, TResult>(this T obj, Func<T, TResult> mapper) => mapper(obj);
}