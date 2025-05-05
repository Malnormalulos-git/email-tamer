using EmailTamer.Database.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace EmailTamer.Database.Entities.Configuration;

public interface IEntityConfiguration<T> : IEntityTypeConfiguration<T>, INonGenericEntityConfiguration
    where T : class, IEntity
{

}