namespace EmailTamer.Core.Persistence;

public sealed class Blob
{
	public required string Name { get; init; }

	public required string ContentType { get; init; }

	public required Stream Content { get; init; }

	public IDictionary<string, string> Metadata { get; init; } = new Dictionary<string, string>();
}