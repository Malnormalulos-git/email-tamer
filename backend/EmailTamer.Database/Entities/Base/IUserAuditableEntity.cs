using EmailTamer.Database.Entities.Base;

namespace EmailTamer.Database.Shared.Entities.Base;

public interface IUserAuditableEntity : IDateAuditableEntity, IEmailTamerUserAuditableEntity;