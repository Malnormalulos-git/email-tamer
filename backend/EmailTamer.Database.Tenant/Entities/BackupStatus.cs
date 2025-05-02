namespace EmailTamer.Database.Tenant.Entities;

public enum BackupStatus
{
    Idle = 0,
    Queued,
    InProgress,
    Failed,
}