namespace EmailTamer.Database.Tenant.Entities;

public enum ConnectionFault
{
    Other = 1,
    ConnectionRefused,
    PortOutOfRange,
    WrongAuthenticationCredentials
}