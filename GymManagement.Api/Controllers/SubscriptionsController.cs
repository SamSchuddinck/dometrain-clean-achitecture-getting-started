using System;
using GymManagement.Application.Services;
using GymManagement.Contracts.Subscriptions;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionsService _subscriptionsService;
    public SubscriptionsController(ISubscriptionsService subscriptionsService)
    {
        _subscriptionsService = subscriptionsService;   
    }

    [HttpPost]
    public IActionResult CreateSubscription(CreateSubscriptionRequest request)
    {
        var subscriptionId = _subscriptionsService.CreateSubscription(request.SubscriptionType.ToString(), request.AdminId);

        var response = new SubscriptionResponse
        {
            Id = subscriptionId,
            SubscriptionType = request.SubscriptionType,
        };

        return Ok(response);
    }
}
