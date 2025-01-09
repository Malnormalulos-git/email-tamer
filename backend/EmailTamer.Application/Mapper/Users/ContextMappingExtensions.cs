using AutoMapper;

namespace EmailTamer.Mapper.Users;

public static class ContextMappingExtensions
{
	public static ContextMappingExpression<TSource, TDestination, UserRoleMappingContext>
		WithUserRoleContext<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expr)
		=> expr.WithContext(ctx => UserRoleMappingContext.FromContext(ctx.Items));

	public static ContextMappingExpression<TSource, TDestination, AuthUserMappingContext>
		WithAuthUserContext<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expr)
		=> expr.WithContext(ctx => AuthUserMappingContext.FromContext(ctx.Items));
}