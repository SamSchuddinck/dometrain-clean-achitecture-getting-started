using System;
using System.Runtime.InteropServices.Marshalling;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Admins.Events;
using MediatR;

namespace GymManagement.Application.Gyms;

public class SubscriptionDeletedEventHandler(IGymsRepository gymsRepository, IUnitOfWork unitOfWork) : INotificationHandler<SubscriptionDeletedEvent>
{
    private readonly IGymsRepository _gymsRepository = gymsRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task Handle(SubscriptionDeletedEvent notification, CancellationToken cancellationToken)
    {
        var gymsToDelete = await _gymsRepository.ListBySubscriptionIdAsync(notification.SubscriptionId)
            ?? throw new InvalidOperationException("Gyms not found for the given subscription ID");

        await _gymsRepository.RemoveRangeAsync(gymsToDelete);
        await _unitOfWork.CommitChangesAsync();
    }
}
