namespace EmailTamer.Database.Entities.Base;

public interface IEmailTamerUserAuditableEntity : IEntity
{
	string CreatedByUserId { get; set; }

	string ModifiedByUserId { get; set; }
}