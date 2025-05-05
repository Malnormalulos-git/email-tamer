using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using AutoMapper;

namespace EmailTamer.Core.Mappers;

public static class MappingExpressionExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IMappingExpression<TSource, TDestination> IgnoreMember<TSource, TDestination, TDestinationMember>(
        this IMappingExpression<TSource, TDestination> expr,
        Expression<Func<TDestination, TDestinationMember>> destinationMember)
        where TSource : class
        where TDestination : class
        => expr.ForMember(destinationMember, x => x.Ignore());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IMappingExpression<TSource, TDestination> IgnoreSourceMember<TSource, TDestination>(
        this IMappingExpression<TSource, TDestination> expr,
        Expression<Func<TSource, object>> memberExpr)
        where TSource : class
        where TDestination : class
        => expr.ForSourceMember(memberExpr, x => x.DoNotValidate());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IMappingExpression<TSource, TDestination> EasyMember<TSource,
                                                                       TDestination,
                                                                       TSourceMember,
                                                                       TDestinationMember>(
        this IMappingExpression<TSource, TDestination> expr,
        Expression<Func<TDestination, TDestinationMember>> destinationMember,
        Expression<Func<TSource, TSourceMember>> sourceMember,
        Action<IMemberConfigurationExpression<TSource, TDestination, TDestinationMember>>? configure = null)
        where TSource : class
        where TDestination : class
        => expr.ForMember(destinationMember, x =>
        {
            x.MapFrom(sourceMember);
            configure?.Invoke(x);
        });

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IMappingExpression<TSource, TDestination> EasyIdMember<TSource,
                                                                         TDestination,
                                                                         TDestinationMember>(
        this IMappingExpression<TSource, TDestination> expr,
        Expression<Func<TDestination, TDestinationMember>> destinationMember,
        Action<IMemberConfigurationExpression<TSource, TDestination, TDestinationMember>>? configure = null)
        where TSource : class
        where TDestination : class
        => expr.EasyMember(destinationMember, x => x, configure);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IMappingExpression<TSource, TDestination> EasyMember<TSource,
                                                                       TSourceMember,
                                                                       TDestination,
                                                                       TDestinationMember>(
        this IMappingExpression<TSource, TDestination> expr,
        Expression<Func<TDestination, TDestinationMember>> destinationMember,
        Func<TSource, TDestination, TSourceMember> sourceMember,
        Action<IMemberConfigurationExpression<TSource, TDestination, TDestinationMember>>? configure = null)
        where TSource : class
        where TDestination : class
        => expr.ForMember(destinationMember, x =>
        {
            x.MapFrom(sourceMember);
            configure?.Invoke(x);
        });

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IMappingExpression<TSource, TDestination> EasyMapperMember<TSource,
                                                                             TDestination,
                                                                             TSourceMember,
                                                                             TDestinationMember>(
        this IMappingExpression<TSource, TDestination> expr,
        Expression<Func<TDestination, TDestinationMember>> destinationMember,
        Func<TSource, IRuntimeMapper, TSourceMember> sourceMember,
        Action<IMemberConfigurationExpression<TSource, TDestination, TDestinationMember>>? configure = null)
        where TSource : class
        where TDestination : class
        => expr.ForMember(destinationMember, x =>
        {
            x.MapFrom((s, _, _, ctx) => sourceMember(s, ctx.Mapper));
            configure?.Invoke(x);
        });

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IMappingExpression<TSource, TDestination> EasyMapperMember<TSource,
                                                                             TDestination,
                                                                             TSourceMember,
                                                                             TDestinationMember>(
        this IMappingExpression<TSource, TDestination> expr,
        Expression<Func<TDestination, TDestinationMember>> destinationMember,
        Func<TSource, TDestination, IRuntimeMapper, TSourceMember> sourceMember,
        Action<IMemberConfigurationExpression<TSource, TDestination, TDestinationMember>>? configure = null)
        where TSource : class
        where TDestination : class
        => expr.ForMember(destinationMember, x =>
        {
            x.MapFrom((s, d, _, ctx) => sourceMember(s, d, ctx.Mapper));
            configure?.Invoke(x);
        });
}