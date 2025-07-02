using System;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GymManagement.Api.IntegrationTests.Common;
using GymManagement.Contracts.Subscriptions;
using TestCommon.TestConstants;

namespace GymManagement.Api.IntegrationTests.Controllers;

[Collection(GymManagementApiFactoryCollection.CollectionName)]

public class CreateSubscriptionTests
{
    private readonly HttpClient _client;

    public CreateSubscriptionTests(GymManagementApiFactory apiFactory)
    {
        _client = apiFactory.HttpClient;
        apiFactory.ResetDatabase();
    }

    [Theory]
    [MemberData(nameof(ListSubscriptionTypes))]
    public async Task CreateSubscription_WhenValidSubscription_ShouldCreateSubscription(SubscriptionType subscriptionType)
    {
        // Arrange
        // Create the CreateSubscriptionRequest
        var createSubscriptionRequest = new CreateSubscriptionRequest
        {
            SubscriptionType = subscriptionType,
            AdminId = Constants.Admin.Id
        };

        // Act

        // Send the request using the httpclient
        var response = await _client.PostAsJsonAsync("Subscriptions", createSubscriptionRequest);

        // Assert
        // Response contains the created subscription
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var subscriptionResponse = await response.Content.ReadFromJsonAsync<SubscriptionResponse>();

        subscriptionResponse.Should().NotBeNull();
        subscriptionResponse.SubscriptionType.Should().Be(subscriptionType);
        response.Headers.Location.PathAndQuery.Should().Be($"/Subscriptions/{subscriptionResponse.Id}");

    }

    public static TheoryData<SubscriptionType> ListSubscriptionTypes()
    {
        var theoryData = new TheoryData<SubscriptionType>();
        foreach (SubscriptionType subscriptionType in Enum.GetValues(typeof(SubscriptionType)))
        {
            theoryData.Add(subscriptionType);
        }
        return theoryData;
    }
}
