using AutoMapper;

namespace EmailTamer.Core.Mappers;

public static class ContextMappingExtensions
{
    public static TDestination MapWithContext<TDestination>(
        this IMapper mapper,
        object? source,
        IMappingContext ctx,
        Action<IDictionary<string, object>>? enrichContext = null)
        => mapper.Map<TDestination>(source, options =>
        {
            ctx.EnrichContext(options.Items);
            enrichContext?.Invoke(options.Items);
        });

    public static TDestination MapWithContext<TSource, TDestination>(
        this IMapper mapper,
        TSource source,
        TDestination destination,
        IMappingContext ctx,
        Action<IDictionary<string, object>>? enrichContext = null)
        => mapper.Map(source, destination, options =>
        {
            ctx.EnrichContext(options.Items);
            enrichContext?.Invoke(options.Items);
        });

    public static object MapWithContext(
        this IMapper mapper,
        object source,
        Type sourceType,
        Type destinationType,
        IMappingContext ctx,
        Action<IDictionary<string, object>>? enrichContext = null)
        => mapper.Map(source, sourceType, destinationType, options =>
        {
            ctx.EnrichContext(options.Items);
            enrichContext?.Invoke(options.Items);
        });

    public static ContextMappingExpression<TSource, TDestination, TContext>
        WithContext<TSource, TDestination, TContext>(this IMappingExpression<TSource, TDestination> expr,
                                                     Func<ResolutionContext, TContext> contextSelector)
        where TContext : IMappingContext
        => new(expr, contextSelector);
}