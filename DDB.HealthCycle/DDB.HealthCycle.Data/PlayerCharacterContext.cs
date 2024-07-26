using DDB.HealthCycle.Models.DataModels;
using Microsoft.EntityFrameworkCore;

namespace DDB.HealthCycle.Data;

public class PlayerCharacterContext(DbContextOptions<PlayerCharacterContext> options) : DbContext(options)
{
    public DbSet<PlayerCharacterRecord> PlayerCharacterRecords { get; set; }
}
