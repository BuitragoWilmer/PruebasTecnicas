using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.TaskStatuses.Persistence.Configurations;

public class TaskStatusConfiguration : IEntityTypeConfiguration<TaskStatus>
{
    public void Configure(EntityTypeBuilder<TaskStatus> builder)
    {

        builder.HasKey(status => status.StatusId);
        
        builder.Property(status => status.StatusId)
            .ValueGeneratedNever();

        builder.Property(status => status.StatusName)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(status => status.StatusName)
            .IsUnique();
    }
}