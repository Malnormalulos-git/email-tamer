using Polly;

namespace EmailTamer.Database.Utilities;

public interface IDatabasePolicySet
{
    IAsyncPolicy DatabaseReadPolicy { get; }
    IAsyncPolicy DatabaseWritePolicy { get; }
}