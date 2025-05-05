namespace EmailTamer.Database.Entities.Base;

public interface IDateSoftDeletableEntity : IEntity
{
    DateTime? DeletedAt { get; set; }
}