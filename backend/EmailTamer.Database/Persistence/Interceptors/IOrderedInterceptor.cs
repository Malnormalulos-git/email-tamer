using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EmailTamer.Database.Persistence.Interceptors;

public interface IOrderedInterceptor : IInterceptor
{
    uint Order { get; }
}