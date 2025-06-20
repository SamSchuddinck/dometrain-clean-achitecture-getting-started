using System;

namespace GymManagement.Contracts.Subscriptions;

public record CreateSubscriptionRequest
{
    public SubscriptionType SubscriptionType { get; init; }
    public Guid AdminId { get; init; }
}
