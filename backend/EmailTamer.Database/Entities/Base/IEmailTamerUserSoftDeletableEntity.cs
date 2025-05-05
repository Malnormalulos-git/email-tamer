namespace EmailTamer.Database.Entities.Base;

public interface IEmailTamerUserSoftDeletableEntity : IEntity
{
    string? DeletedByUserId { get; set; }
}