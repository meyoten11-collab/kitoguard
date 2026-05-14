using Database.VSRO188.SRO_VT_SHARD;
using Microsoft.EntityFrameworkCore;

namespace Database.VSRO188.Context;

public partial class SRO_VT_SHARD
{
    public virtual DbSet<ServerAddonGameServerAction> ServerAddonGameServerActions { get; set; }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ServerAddonGameServerAction>(entity =>
        {
            entity.HasKey(e => e.ID);

            entity.ToTable("_ExeGameServer");

            entity.Property(e => e.CharName16)
                .HasMaxLength(64)
                .IsUnicode(false);

            entity.Property(e => e.Param01)
                .HasMaxLength(129)
                .IsUnicode(false);
        });
    }
}
