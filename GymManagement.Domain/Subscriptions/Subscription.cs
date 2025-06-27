using System;
using System.Reflection.Emit;
using ErrorOr;
using GymManagement.Domain.Gyms;
using Throw;

namespace GymManagement.Domain.Subscriptions;

public class Subscription
{
    private readonly List<Guid> _gymIds = [];

    private readonly int _maxGyms;
    private readonly Guid _adminId;
    public Guid Id { get; private set; }

    public SubscriptionType SubscriptionType { get; private set; }

    public Subscription(SubscriptionType subscriptionType, Guid adminId, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        SubscriptionType = subscriptionType;
        _adminId = adminId;

        _maxGyms = GetMaxGyms();
    }

    private Subscription() { }

    public ErrorOr<Success> AddGym(Gym gym)
    {
        _gymIds.Throw().IfContains(gym.Id);

        if (_gymIds.Count >= _maxGyms)
        {
            return SubscriptionErrors.CannotHaveMoreGymsThanTheSubscriptionAllows;
        }

        _gymIds.Add(gym.Id);

        return Result.Success;
    }

    public int GetMaxGyms() => SubscriptionType.Name switch
    {
        nameof(SubscriptionType.Free) => 1,
        nameof(SubscriptionType.Starter) => 1,
        nameof(SubscriptionType.Pro) => 3,
        _ => throw new InvalidOperationException()
    };
    
     public void RemoveGym(Guid gymId)
    {
        _gymIds.Throw().IfNotContains(gymId);

        _gymIds.Remove(gymId);
    }
}
