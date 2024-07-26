namespace DDB.HealthCycle.DataAccess.DateTimeProvider;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
