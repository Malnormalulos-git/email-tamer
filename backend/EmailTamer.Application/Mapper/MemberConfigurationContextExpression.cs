using AutoMapper;

namespace EmailTamer.Mapper;

public sealed class MemberConfigurationContextExpression
	<TSource, TDestination, TDestinationMember, TContext, TContextMember>(
		IMemberConfigurationExpression<TSource, TDestination, TDestinationMember> memberConfigurationExpression,
		Func<ResolutionContext, TContext> contextSelector,
		Func<TContext, TContextMember> contextMemberSelector
	)
{
	public void PreCondition(Func<TSource, TContextMember, bool> condition)
		=> memberConfigurationExpression.PreCondition((src, ctx)
			=> condition.Invoke(src, contextMemberSelector(contextSelector(ctx))));

	public void Condition(Func<TSource, TDestination, TDestinationMember, TContextMember, bool> condition)
		=> memberConfigurationExpression.Condition((src, dest, mapped, _, ctx)
			=> condition.Invoke(src, dest, mapped, contextMemberSelector(contextSelector(ctx))));

	public void SetMappingOrder(int mappingOrder) => memberConfigurationExpression.SetMappingOrder(mappingOrder);
}