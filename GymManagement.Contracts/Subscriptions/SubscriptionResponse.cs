namespace GymManagement.Contracts.Subscriptions;

public record class SubscriptionResponse
{
    public Guid Id { get; init; }
    public SubscriptionType SubscriptionType { get; init; }
}
