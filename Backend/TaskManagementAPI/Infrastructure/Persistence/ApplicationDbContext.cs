

using Application.Data;
using Domain;
using Domain.Primitives;
using Domain.TaskItems;
using Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext, IUnitOfWork
{
    private readonly IPublisher? _publisher;
    public ApplicationDbContext(DbContextOptions options, IPublisher? publisher = null) : base(options)
    {
        _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
    }

    
    public DbSet<TaskItem> TaskItems { get ; set ;}
    public DbSet<User> Users { get ; set ;}
    public DbSet<TaskStatus> TaskStatuses { get; set; }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        var domainEvents = ChangeTracker.Entries<AggregateRoot>()
            .Select(e => e.Entity)
            .SelectMany(e =>
            {
                var events = e.GetDomainEvents();
                e.ClearDomainEvents();
                return events;
            })
            .ToList();
        
        await base.SaveChangesAsync(cancellationToken); // Guarda los eventos en la misma transacción

        return result;
    }
}