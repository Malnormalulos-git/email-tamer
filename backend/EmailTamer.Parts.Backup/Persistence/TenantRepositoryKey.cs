namespace EmailTamer.Parts.Sync.Persistence;

public abstract class TenantRepositoryKey
{
	public string MessageId { get; init; } = null!;

	public static implicit operator string(TenantRepositoryKey key) => key.ToString();

	public override string ToString()
		=> $"{Uri.EscapeDataString(MessageId)}/";
}