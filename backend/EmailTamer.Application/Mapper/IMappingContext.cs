namespace EmailTamer.Mapper;

public interface IMappingContext
{
	IDictionary<string, object> EnrichContext(IDictionary<string, object> context);
}