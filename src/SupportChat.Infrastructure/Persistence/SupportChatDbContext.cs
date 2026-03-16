using Microsoft.EntityFrameworkCore;

namespace SupportChat.Infrastructure.Persistence;

public class SupportChatDbContext : DbContext
{
    public SupportChatDbContext(DbContextOptions<SupportChatDbContext> options)
        : base(options)
    {
    }

    public DbSet<ChatSessionRecord> ChatSessions => Set<ChatSessionRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatSessionRecord>(entity =>
        {
            entity.ToTable("ChatSessions");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Status)
                .IsRequired();

            entity.Property(x => x.CreatedAtUtc)
                .IsRequired();

            entity.Property(x => x.LastPolledAtUtc);

            entity.Property(x => x.AssignedAgentId);

            entity.Property(x => x.CorrelationId);
        });
    }
}