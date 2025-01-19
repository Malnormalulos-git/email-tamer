using System.Reflection;
using EmailTamer.Database.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static System.Linq.Expressions.Expression;

namespace EmailTamer.Database.Entities.Configuration;

public abstract class EntityConfiguration<T> : IEntityConfiguration<T>
	where T : class, IEntity
{
	public virtual void Configure(EntityTypeBuilder<T> builder)
	{
		var type = typeof(T);
		var commentAttribute = type.GetCustomAttribute<CommentAttribute>();

		if (commentAttribute != null)
		{
			builder.Metadata.SetComment(commentAttribute.Comment);
		}

		if (builder.Metadata.BaseType == null)
		{
			var indexPropertyNames = new List<string>(3);

			void MakeAuditDate(PropertyBuilder property)
			{
				// 6 bytes per date, just for economy - dates won't have millis;
				property.HasPrecision(0).HasColumnType("DATETIME");
			}

			if (type.IsAssignableTo(typeof(IDateAuditableEntity)))
			{
				MakeAuditDate(builder.Property(nameof(IDateAuditableEntity.CreatedAt)));
				MakeAuditDate(builder.Property(nameof(IDateAuditableEntity.ModifiedAt)));
				indexPropertyNames.AddRange(new[]
				{
					nameof(IDateAuditableEntity.CreatedAt),
					nameof(IDateAuditableEntity.ModifiedAt)
				});
			}

			if (type.IsAssignableTo(typeof(IDateSoftDeletableEntity)))
			{
				MakeAuditDate(builder.Property(nameof(IDateSoftDeletableEntity.DeletedAt)));
				var parameter = Parameter(type);
				var deletedAt = MakeMemberAccess(parameter,
					type.GetProperty(nameof(IDateSoftDeletableEntity.DeletedAt))!);
				var eq = Equal(deletedAt, Constant(null));
				var expression = Lambda<Func<T, bool>>(eq, parameter);
				builder.HasQueryFilter(expression);
				indexPropertyNames.Add(nameof(IDateSoftDeletableEntity.DeletedAt));
			}

			if (indexPropertyNames.Count != 0)
			{
				builder.HasIndex(indexPropertyNames.ToArray());
			}

			if (type.IsAssignableTo(typeof(UniqueIdEntity)))
			{
				foreach (var k in builder.Metadata.GetKeys())
				{
					if (k.Properties.Count != 1)
					{
						throw new InvalidOperationException("Unique id entity should have a single surrogate key");
					}

					var keyProperty = k.Properties[0];
					keyProperty.ValueGenerated = ValueGenerated.Never;
					var name = keyProperty.Name;
					var constraintName = $"CHK_{builder.Metadata.GetTableName()}_{name}NotDefault";
					if (builder.Metadata.FindCheckConstraint(constraintName) == null)
					{
						builder.Metadata.AddCheckConstraint(
							constraintName,
							$"""
							 "{name}" <> '00000000-0000-0000-0000-000000000000'
							 """);
					}
				}
			}
		}

		foreach (var mutableNavigation in builder.Metadata.GetNavigations())
		{
			mutableNavigation.ForeignKey.DeleteBehavior = DeleteBehavior.ClientCascade;
		}
	}
}