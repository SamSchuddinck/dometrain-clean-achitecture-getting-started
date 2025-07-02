using System;
using System.Reflection;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Admins;
using GymManagement.Domain.Common;
using GymManagement.Domain.Gyms;
using GymManagement.Domain.Subscriptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Infrastructure.Common.Persistence;

public class GymManagementDbContext(DbContextOptions<GymManagementDbContext> options, IHttpContextAccessor httpContextAccessor, IPublisher _publisher) : DbContext(options), IUnitOfWork
{
    public DbSet<Admin> Admins { get; set; } = null!;

    public DbSet<Subscription> Subscriptions { get; set; } = null!;
    public DbSet<Gym> Gyms { get; set; } = null!;

    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;


    public async Task CommitChangesAsync()
    {
        // Get hold of all the domain events
        var domainEvents = ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity.PopDomainEvents())
            .SelectMany(events => events)
            .ToList();

            
        // Store them in the http context for later
        if (IsUserWaitingOnline())
        {
            AddDomainEventsToOfflineProcessingQueue(domainEvents);
        }
        else
        {
            await PublishDomainEvents(_publisher, domainEvents);
        }

        await SaveChangesAsync();
    }

    private async Task PublishDomainEvents(IPublisher publisher, List<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            await publisher.Publish(domainEvent);
        }
    }

    private bool IsUserWaitingOnline() => _httpContextAccessor.HttpContext is not null;

    private void AddDomainEventsToOfflineProcessingQueue(List<IDomainEvent> domainEvents)
    {
        // Fetch queue from http context or create a new one if it doesn't exist
        var domainEventsQueue = _httpContextAccessor.HttpContext!.Items
        .TryGetValue("DomainEventsQueue", out var queue) && queue is Queue<IDomainEvent> existingQueue
        ? existingQueue
        : new Queue<IDomainEvent>();

        //Add domain events to end of queue
        domainEvents.ForEach(domainEventsQueue.Enqueue);

        // Store the updated queue back in the http context
        _httpContextAccessor.HttpContext!.Items["DomainEventsQueue"] = domainEventsQueue;

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
