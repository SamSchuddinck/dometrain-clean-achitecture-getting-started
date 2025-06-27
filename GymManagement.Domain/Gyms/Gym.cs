using System;
using ErrorOr;

namespace GymManagement.Domain.Gyms;

public class Gym
{
    private readonly int _maxRooms;
    public Guid Id { get; }
    public string Name { get; init; } = null!;

    public Guid SubscriptionId { get; init; }

    private readonly List<Guid> _roomIds = [];
    private readonly List<Guid> _trainerIds = [];

    public Gym(
        string name,
        int maxRooms,
        Guid subscriptionId,
        Guid? id = null
    )
    {
        Name = name;
        _maxRooms = maxRooms;
        SubscriptionId = subscriptionId;
        Id = id ?? Guid.NewGuid();
    }

    public ErrorOr<Success> AddTrainer(Guid trainerId)
    {
        if (_trainerIds.Contains(trainerId))
        {
            return Error.Conflict(description: "Trainer already added to gym");
        }

        _trainerIds.Add(trainerId);
        return Result.Success;
    }

    public bool HasTrainer(Guid trainerId)
    {
        return _trainerIds.Contains(trainerId);
    }

    private Gym() {}
}
