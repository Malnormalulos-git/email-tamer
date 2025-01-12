using System.Linq.Expressions;
using FastExpressionCompiler;
using static System.Linq.Expressions.Expression;

namespace EmailTamer.Core.Extensions;

public static class ExpressionExtensions
{
	public static Action<T, TProperty> CreateSetter<T, TProperty>(this Expression<Func<T, TProperty>> getter)
	{
		var memberExpr = (MemberExpression)getter.Body;
		var @this = Parameter(typeof(T), "$this");
		var value = Parameter(typeof(TProperty), "value");
		return Lambda<Action<T, TProperty>>(
			Assign(MakeMemberAccess(@this, memberExpr.Member), value),
			@this, value).CompileFast();
	}
}