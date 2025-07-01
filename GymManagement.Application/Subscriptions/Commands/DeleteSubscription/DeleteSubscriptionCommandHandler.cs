
using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using MediatR;

namespace GymManagement.Application.Subscriptions.Commands.DeleteSubscription;

public class DeleteSubscriptionCommandHandler : IRequestHandler<DeleteSubscriptionCommand, ErrorOr<Deleted>>
{
    private readonly ISubscriptionsRepository _subscriptionRepository;
    private readonly IAdminsRepository _adminsRepository;


    private readonly IUnitOfWork _unitOfWork;

    public DeleteSubscriptionCommandHandler(
        ISubscriptionsRepository subscriptionRepository,
        IAdminsRepository adminsRepository,

        IUnitOfWork unitOfWork)
    {
        _subscriptionRepository = subscriptionRepository;
        _adminsRepository = adminsRepository;

        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Deleted>> Handle(DeleteSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(request.SubscriptionId);

        if (subscription is null)
        {
            return Error.NotFound(description: "Subscription not found.");
        }

        var admin = await _adminsRepository.GetByIdAsync(subscription.AdminId);

        if (admin is null)
        {
            return Error.Unexpected(description: "Admin not found");
        }

        admin.DeleteSubscription(request.SubscriptionId);

        await _adminsRepository.UpdateAsync(admin);
        await _unitOfWork.CommitChangesAsync();

        return Result.Deleted;
    }
}