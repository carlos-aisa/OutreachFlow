using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OutreachFlow.Infrastructure.Persistence.DesignTime;

public sealed class OutreachFlowDbContextFactory : IDesignTimeDbContextFactory<OutreachFlowDbContext>
{
    private const string DefaultConnectionString = "Data Source=outreachflow.db";

    public OutreachFlowDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OutreachFlowDbContext>();
        optionsBuilder.UseSqlite(DefaultConnectionString);

        return new OutreachFlowDbContext(optionsBuilder.Options);
    }
}
