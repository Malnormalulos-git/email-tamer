using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace EmailTamer.Core.Extensions;

public static class QueryableExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, bool condition, Expression<Func<T, bool>> predicate)
        => condition
            ? source.Where(predicate)
            : source;
}