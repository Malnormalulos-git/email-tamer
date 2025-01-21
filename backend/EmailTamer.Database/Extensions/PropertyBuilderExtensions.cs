using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace EmailTamer.Database.Extensions;

public static class PropertyBuilderExtensions
{
	public static PropertyBuilder<DateTime> DateTime(this PropertyBuilder<DateTime> builder)
		=> builder.HasPrecision(0).HasColumnType("DATETIME");

	public static PropertyBuilder<DateTime?> DateTime(this PropertyBuilder<DateTime?> builder)
		=> builder.HasPrecision(0).HasColumnType("DATETIME");

	public static PropertyBuilder<T> Json<T>(this PropertyBuilder<T> builder) where T : new()
		=> builder.HasConversion(
			v => JsonConvert.SerializeObject(v),
			v => JsonConvert.DeserializeObject<T>(v) ?? new T());
}