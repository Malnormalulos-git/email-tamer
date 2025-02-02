using EmailTamer.Database.Utilities.Paging;
using Microsoft.EntityFrameworkCore;

namespace EmailTamer.Database.Extensions;

public static class QueryableExtensions
{
	private static IQueryable<T> ApplyPaging<T>(this IQueryable<T> source, int size, int page)
		=> source
			.Skip(size * page)
			.Take(size);

	public static async Task<PagedResult<T>> ToPagedResultAsync<T>(this IQueryable<T> source, IPagedRequest request,
	                                                               CancellationToken cancellationToken = default)
	{
		var count = await source.CountAsync(cancellationToken);
		var data = await source
			.ApplyPaging(request.Size, request.Page - 1)
			.ToListAsync(cancellationToken);
		return new (data, request.Page, request.Size, count);
	}
}