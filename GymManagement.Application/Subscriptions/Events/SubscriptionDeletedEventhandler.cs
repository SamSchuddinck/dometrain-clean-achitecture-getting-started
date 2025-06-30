using System;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Admins.Events;
using GymManagement.Domain.Subscriptions;
using MediatR;

namespace GymManagement.Application.Subscriptions.Events;

public class SubscriptionDeletedEventHandler(ISubscriptionsRepository subscriptionsRepository, IUnitOfWork unitOfWork) : INotificationHandler<SubscriptionDeletedEvent>
{
    private readonly ISubscriptionsRepository _subscriptionsRepository = subscriptionsRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(SubscriptionDeletedEvent notification, CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionsRepository.GetByIdAsync(notification.SubscriptionId) ?? throw new InvalidOperationException("Subscription not found");

        await _subscriptionsRepository.DeleteSubscriptionAsync(subscription);
        await _unitOfWork.CommitChangesAsync();
    }
}
