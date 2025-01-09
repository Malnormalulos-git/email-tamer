namespace EmailTamer.Core.Mappers;

public interface IMappingContext
{
	IDictionary<string, object> EnrichContext(IDictionary<string, object> context);
}