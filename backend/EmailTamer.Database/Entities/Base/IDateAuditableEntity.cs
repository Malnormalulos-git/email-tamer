namespace EmailTamer.Database.Entities.Base;

public interface IDateAuditableEntity : IEntity
{
    DateTime CreatedAt { get; set; }

    DateTime ModifiedAt { get; set; }
}