using System;
using Ardalis.SmartEnum;

namespace GymManagement.Domain.Subscriptions;

public class SubscriptionType: SmartEnum<SubscriptionType>
{
    public static readonly SubscriptionType Free = new(0, nameof(Free));
    public static readonly SubscriptionType Starter = new(1, nameof(Starter));
    public static readonly SubscriptionType Pro = new(2, nameof(Pro));

    private SubscriptionType(int value, string name) : base(name, value)
    {
    }
}
