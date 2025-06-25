using ErrorOr;
using MediatR;

namespace GymManagement.Application.Subscriptions.Commands.CreateSubscription;
public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, ErrorOr<Guid>>
{
    public async Task<ErrorOr<Guid>> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        return Guid.NewGuid(); // Simulate async operation, replace with actual logic to create a subscription
    }
}