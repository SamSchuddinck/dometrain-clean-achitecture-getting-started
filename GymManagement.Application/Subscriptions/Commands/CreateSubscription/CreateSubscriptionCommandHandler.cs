using MediatR;

namespace GymManagement.Application.Subscriptions.Commands.CreateSubscription;
public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, Guid>
{
    public Task<Guid> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Guid.NewGuid()); // Simulate async operation, replace with actual logic to create a subscription
    }
}