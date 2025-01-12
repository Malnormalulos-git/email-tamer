using JetBrains.Annotations;

namespace EmailTamer.Database.Utilities.Paging;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public sealed record PagedResult<T>(IReadOnlyCollection<T> Items, int Page, int Size, int Total)
{
}