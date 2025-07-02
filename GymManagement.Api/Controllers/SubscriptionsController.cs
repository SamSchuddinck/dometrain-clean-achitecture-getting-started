using GymManagement.Contracts.Subscriptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using GymManagement.Application.Subscriptions.Commands.CreateSubscription;
using GymManagement.Application.Subscriptions.Queries.GetSubscription;
using DomainSubcriptionType = GymManagement.Domain.Subscriptions.SubscriptionType;
using GymManagement.Application.Subscriptions.Commands.DeleteSubscription;

namespace GymManagement.Api.Controllers;


[Route("[controller]")]
public class SubscriptionsController : ApiController
{
    private readonly ISender _mediator;
    public SubscriptionsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSubscription(CreateSubscriptionRequest request)
    {

        if (!DomainSubcriptionType.TryFromName(request.SubscriptionType.ToString(), out var subscriptionType))
        {
            return Problem(
                detail: $"Invalid subscription type: {request.SubscriptionType}",
                statusCode: StatusCodes.Status400BadRequest
            );
        }

        var command = new CreateSubscriptionCommand(
            subscriptionType,
            request.AdminId
        );

        var createSubscriptionResult = await _mediator.Send(command);

        return createSubscriptionResult.MatchFirst(
            subscription => CreatedAtAction(
                nameof(GetSubscription),
                new {subscriptionId = subscription.Id},
                new SubscriptionResponse
                {
                    Id = subscription.Id,
                    SubscriptionType = ToDto(subscription.SubscriptionType),
                }),
            Problem
        );
    }

    [HttpGet("{subscriptionId:guid}")]
    public async Task<IActionResult> GetSubscription(Guid subscriptionId)
    {
        var query = new GetSubscriptionQuery(subscriptionId);

        var getSubscriptionResult = await _mediator.Send(query);

        return getSubscriptionResult.MatchFirst(
            subscription => Ok(new SubscriptionResponse
            {
                Id = subscription.Id,
                SubscriptionType = ToDto(subscription.SubscriptionType)
            }),
            Problem
        );
    }

    [HttpDelete("{subscriptionId:guid}")]
    public async Task<IActionResult> DeleteSubscription(Guid subscriptionId)
    {
        var command = new DeleteSubscriptionCommand(subscriptionId);
        var deleteSubscriptionResult = await _mediator.Send(command);

        return deleteSubscriptionResult.MatchFirst<IActionResult>(
            _ => NoContent(),
            Problem
        );
    }

    private static SubscriptionType ToDto(DomainSubcriptionType subscriptionType)
    {
        return subscriptionType.Name switch
        {
            nameof(DomainSubcriptionType.Free) => SubscriptionType.Free,
            nameof(DomainSubcriptionType.Starter) => SubscriptionType.Starter,
            nameof(DomainSubcriptionType.Pro) => SubscriptionType.Pro,
            _ => throw new InvalidOperationException(),
        };
    }    
}
