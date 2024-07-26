namespace DDB.HealthCycle.Models.DataModels;
public abstract class DbModelBase
{
    public DateTime Updated { get; set; }
    // I would normally add a created, as well as log the use making the change
    // but this there is no user data or intention to create new data in this test app
}
