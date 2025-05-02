namespace EmailTamer.Database.Tenant.Entities;

public enum BackupStatus
{
    Idle = 1,
    Queued,
    InProgress,
    Failed,
}