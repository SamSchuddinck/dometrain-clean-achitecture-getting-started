using System.Runtime.CompilerServices;
using ErrorOr;
using FluentAssertions;
using GymManagement.Domain.Subscriptions;
using Microsoft.VisualStudio.TestPlatform.CrossPlatEngine.Helpers;
using TestCommon.Gyms;
using TestCommon.Subscriptions;

namespace  GymManagement.Domain.UnitTests.Subscriptions;

public class SubscriptionTests
{
    [Fact]
    public void AddGym_WhenMoreThanSubscriptionAllows_ShouldFail()
    {
        // Arrange
        // Create a subscription
        var subscription = SubscriptionFactory.CreateSubscription();
        // Create max number of gyms + 1
        var gyms = Enumerable.Range(0, subscription.GetMaxGyms() + 1)
                .Select(_ => GymFactory.CreateGym(id: Guid.NewGuid()))
                .ToList();
        // Act
        var addGymResults = gyms.ConvertAll(subscription.AddGym);

        // Assert
        // Adding all the gyms succeeded, but the last failed
        var allButLastGymResults = addGymResults[..^1];
        var lastAddGymResult = addGymResults.Last();

        allButLastGymResults.Should().AllSatisfy(addGymResult => addGymResult.Value.Should().Be(Result.Success));

        lastAddGymResult.IsError.Should().BeTrue();
        lastAddGymResult.FirstError.Should
        ().Be(SubscriptionErrors.CannotHaveMoreGymsThanTheSubscriptionAllows);

    }    
}
