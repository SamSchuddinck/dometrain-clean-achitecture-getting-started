
using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using MediatR;

namespace GymManagement.Application.Subscriptions.Commands.DeleteSubscription;

public class DeleteSubscriptionCommandHandler : IRequestHandler<DeleteSubscriptionCommand, ErrorOr<Deleted>>
{
    private readonly ISubscriptionsRepository _subscriptionRepository;
    private readonly IAdminsRepository _adminsRepository;

    private readonly IGymsRepository _gymsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSubscriptionCommandHandler(
        ISubscriptionsRepository subscriptionRepository,
        IAdminsRepository adminsRepository,
        IGymsRepository gymsRepository,
        IUnitOfWork unitOfWork)
    {
        _subscriptionRepository = subscriptionRepository;
        _adminsRepository = adminsRepository;
        _gymsRepository = gymsRepository;
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

         var gymsToDelete = await _gymsRepository.ListBySubscriptionIdAsync(request.SubscriptionId);

        await _adminsRepository.UpdateAsync(admin);
        await _subscriptionRepository.DeleteSubscriptionAsync(subscription);
        await _gymsRepository.RemoveRangeAsync(gymsToDelete);
        await _unitOfWork.CommitChangesAsync();

        return Result.Deleted;
    }
}