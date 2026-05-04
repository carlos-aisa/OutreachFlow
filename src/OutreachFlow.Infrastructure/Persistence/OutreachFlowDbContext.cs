using Microsoft.EntityFrameworkCore;

namespace OutreachFlow.Infrastructure.Persistence;

public sealed class OutreachFlowDbContext(DbContextOptions<OutreachFlowDbContext> options)
    : DbContext(options)
{
}
