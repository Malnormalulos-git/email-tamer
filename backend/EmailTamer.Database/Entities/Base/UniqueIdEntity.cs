using System.ComponentModel.DataAnnotations;

namespace EmailTamer.Database.Entities.Base;

public abstract class UniqueIdEntity : IEntity
{
	[Key]
	public Guid Id { get; set; }
}