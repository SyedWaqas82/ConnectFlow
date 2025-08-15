// using ConnectFlow.Domain.Entities;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;

// namespace ConnectFlow.Infrastructure.Data.Configurations;

// public class AIUsageConfiguration : BaseAuditableConfiguration<AIUsage>
// {
//     public override void Configure(EntityTypeBuilder<AIUsage> builder)
//     {
//         base.Configure(builder);

//         // Configure properties
//         builder.Property(ai => ai.ModelId).IsRequired().HasMaxLength(100);
//         builder.Property(ai => ai.ModelName).IsRequired().HasMaxLength(100);
//         builder.Property(ai => ai.InputTokens).IsRequired();
//         builder.Property(ai => ai.OutputTokens).IsRequired();
//         builder.Property(ai => ai.Cost).HasPrecision(18, 2).IsRequired();
//         builder.Property(ai => ai.RequestType).IsRequired();
//         builder.Property(ai => ai.IsSuccessful).IsRequired();
//         builder.Property(ai => ai.ErrorMessage).HasMaxLength(500);

//         // Configure User relationship
//         builder.HasOne(ai => ai.User).WithMany().HasForeignKey(ai => ai.UserId).OnDelete(DeleteBehavior.SetNull);
//         builder.HasOne(ai => ai.Conversation).WithMany().HasForeignKey(ai => ai.ConversationId).OnDelete(DeleteBehavior.SetNull);
//         builder.HasOne(ai => ai.Message).WithMany().HasForeignKey(ai => ai.MessageId).OnDelete(DeleteBehavior.SetNull);
//     }
// }