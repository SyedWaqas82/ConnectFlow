using ConnectFlow.Domain.Common;
using ConnectFlow.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public abstract class BaseAuditableConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseAuditableEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedOnAdd();
        builder.HasIndex(t => t.PublicId).IsUnique();
        //builder.Property(t => t.PublicId).HasDefaultValueSql("newid()").IsRequired(); //SQL Server
        builder.Property(t => t.PublicId).HasDefaultValueSql("gen_random_uuid()").IsRequired(); //PostgreSQL

        // Common navigation properties
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(t => t.CreatedBy).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(t => t.LastModifiedBy).OnDelete(DeleteBehavior.SetNull);
    }
}