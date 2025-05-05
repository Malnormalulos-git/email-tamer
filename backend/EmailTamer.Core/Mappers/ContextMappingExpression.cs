using System.Linq.Expressions;
using AutoMapper;

namespace EmailTamer.Core.Mappers;

public sealed class ContextMappingExpression<TSource, TDestination, TContext>(
    IMappingExpression<TSource, TDestination> expression,
    Func<ResolutionContext, TContext> contextSelector)
{
    public IMappingExpression<TSource, TDestination> Expression { get; } = expression;

    private ContextMemberExpression<TDestinationMember, TContextMember>
        ContextMember<TDestinationMember, TContextMember>(
            Expression<Func<TDestination, TDestinationMember>> destinationExpr,
            Func<TContext, TContextMember> contextMemberSelector)
        => new(Expression, destinationExpr, contextSelector, contextMemberSelector);

    public ContextMappingExpression<TSource, TDestination, TContext>
        EasyContextMember<TDestinationMember, TContextMember>(
            Expression<Func<TDestination, TDestinationMember>> destinationExpr,
            Func<TContext, TContextMember> contextMemberSelector,
            Action<MemberConfigurationContextExpression<TSource, TDestination, TDestinationMember, TContext, TContextMember>>? configure = null)
        => ContextMember(destinationExpr, contextMemberSelector)
            .Project((_, ctxValue) => ctxValue, configure);

    public ContextMappingExpression<TSource, TDestination, TContext>
        EasyContextIdMember<TDestinationMember>(
            Expression<Func<TDestination, TDestinationMember>> destinationExpr,
            Action<MemberConfigurationContextExpression<TSource, TDestination, TDestinationMember, TContext, TContext>>? configure = null)
        => ContextMember(destinationExpr, x => x)
            .Project((_, ctxValue) => ctxValue, configure);

    public ContextMappingExpression<TSource, TDestination, TContext>
        EasyContextMember<TDestinationMember, TContextMember>(
            Expression<Func<TDestination, TDestinationMember>> destinationExpr,
            Func<TContext, TContextMember> contextMemberSelector,
            Func<TSource, TContextMember, TDestinationMember> projector,
            Action<MemberConfigurationContextExpression<TSource, TDestination, TDestinationMember, TContext, TContextMember>>? configure = null)
        => ContextMember(destinationExpr, contextMemberSelector)
            .Project(projector, configure);

    public ContextMappingExpression<TSource, TDestination, TContext>
        EasyContextMember<TDestinationMember, TMappedMember>(
            Expression<Func<TDestination, TDestinationMember>> destinationExpr,
            Func<TSource, TContext, TMappedMember> projector,
            Action<MemberConfigurationContextExpression<TSource, TDestination, TDestinationMember, TContext, TContext>>? configure = null)
        => ContextMember(destinationExpr, x => x)
            .Project(projector, configure);

    public ContextMappingExpression<TSource, TDestination, TContext>
        EasyContextMember<TDestinationMember, TMappedMember>(
            Expression<Func<TDestination, TDestinationMember>> destinationExpr,
            Func<TSource, TDestination, TContext, TMappedMember> projector,
            Action<MemberConfigurationContextExpression<TSource, TDestination, TDestinationMember, TContext, TContext>>? configure = null)
        => ContextMember(destinationExpr, x => x)
            .Project(projector, configure);

    public ContextMappingExpression<TSource, TDestination, TContext> ConvertUsingContext(
        Func<TSource, TContext, TDestination> convert)
    {
        Expression.ConvertUsing((src, _, resolutionCtx) =>
        {
            var ctx = contextSelector(resolutionCtx);
            return convert(src, ctx);
        });

        return this;
    }

    private sealed class ContextMemberExpression<TDestinationMember, TContextMember>(
        IMappingExpression<TSource, TDestination> expression,
        Expression<Func<TDestination, TDestinationMember>> destinationExpr,
        Func<ResolutionContext, TContext> contextSelector,
        Func<TContext, TContextMember> contextMemberSelector)
    {
        public ContextMappingExpression<TSource, TDestination, TContext> Project<TMappedMember>(
            Func<TSource, TContextMember, TMappedMember> projector,
            Action<MemberConfigurationContextExpression<TSource, TDestination, TDestinationMember, TContext, TContextMember>>? configure = null)
        {
            var result = expression.ForMember(destinationExpr, x =>
            {
                x.MapFrom((src, _, _, ctx) =>
                {
                    var ctxMember = contextMemberSelector(contextSelector(ctx));
                    return projector(src, ctxMember);
                });
                configure?.Invoke(new(x, contextSelector, contextMemberSelector));
            });
            return new(result, contextSelector);
        }

        public ContextMappingExpression<TSource, TDestination, TContext> Project<TMappedMember>(
            Func<TSource, TDestination, TContextMember, TMappedMember> projector,
            Action<MemberConfigurationContextExpression<TSource, TDestination, TDestinationMember, TContext, TContextMember>>? configure = null)
        {
            var result = expression.ForMember(destinationExpr, x =>
            {
                x.MapFrom((src, d, _, ctx) =>
                {
                    var ctxMember = contextMemberSelector(contextSelector(ctx));
                    return projector(src, d, ctxMember);
                });
                configure?.Invoke(new(x, contextSelector, contextMemberSelector));
            });
            return new(result, contextSelector);
        }
    }
}