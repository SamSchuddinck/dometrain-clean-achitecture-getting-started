using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Subscriptions;
using MediatR;

namespace GymManagement.Application.Subscriptions.Commands.CreateSubscription;
public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, ErrorOr<Subscription>>
{
    private readonly ISubscriptionsRepository _subscriptionRepository; 
    private readonly IAdminsRepository _adminsRepository;

    private readonly IUnitOfWork _unitOfWork;

    public CreateSubscriptionCommandHandler(ISubscriptionsRepository subscriptionRepository, IAdminsRepository adminsRepository, IUnitOfWork unitOfWork)
    {
        _subscriptionRepository = subscriptionRepository;
        _adminsRepository = adminsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Subscription>> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        // Check if the admin exists
        var admin = await _adminsRepository.GetByIdAsync(request.AdminId);

        if (admin is null)
        {
            return Error.NotFound(description: "Admin not found");
        }

        // Create Subscription
        var subscription = new Subscription(request.SubscriptionType, request.AdminId);

         if (admin.SubscriptionId is not null)
        {
            return Error.Conflict(description: "Admin already has an active subscription");
        }

        admin.SetSubscription(subscription);

        // Add it to the database
        await _subscriptionRepository.AddSubscriptionAsync(subscription);
        await _adminsRepository.UpdateAsync(admin);
        await _unitOfWork.CommitChangesAsync();

        // Return the created subscription
        return subscription;
    }
}