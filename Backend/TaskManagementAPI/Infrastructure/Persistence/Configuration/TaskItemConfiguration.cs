using Domain.TaskItems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {

        builder.HasKey(taskItem => taskItem.TaskId);

        builder.Property(taskItem => taskItem.TaskId)
            .ValueGeneratedOnAdd();

        builder.Property(taskItem => taskItem.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(taskItem => taskItem.Description)
            .HasMaxLength(1000);

        builder.Property(taskItem => taskItem.AssignedUserId)
            .IsRequired();

        builder.Property(taskItem => taskItem.StatusId)
            .IsRequired();

        builder.Property(taskItem => taskItem.AdditionalInfo);

        builder.Property(taskItem => taskItem.CreatedAt)
            .IsRequired();

        builder.Property(taskItem => taskItem.UpdatedAt)
            .IsRequired(false);

        builder.Ignore(taskItem => taskItem.Priority);
        builder.Ignore(taskItem => taskItem.DueDate);
        builder.Ignore(taskItem => taskItem.Status);

        builder.HasOne(taskItem => taskItem.AssignedUser)
            .WithMany()
            .HasForeignKey(taskItem => taskItem.AssignedUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(taskItem => taskItem.Status)
            .WithMany()
            .HasForeignKey(taskItem => taskItem.StatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}