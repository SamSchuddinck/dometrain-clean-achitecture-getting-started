using System;
using System.Threading.Tasks;
using ErrorOr;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using GymManagement.Application.Common.Interfaces.Behaviours;
using GymManagement.Application.Gyms.Commands.CreateGym;
using GymManagement.Domain.Gyms;
using MediatR;
using NSubstitute;
using TestCommon.Gyms;

namespace GymManagement.Application.UnitTests.Common.Behaviours;

public class ValidationBehaviourTests
{
    private readonly ValidationBehaviour<CreateGymCommand, ErrorOr<Gym>> _validationBehaviour;

    private readonly IValidator<CreateGymCommand> _mockValidator;

    private readonly RequestHandlerDelegate<ErrorOr<Gym>> _mockNextBehaviour;

    public ValidationBehaviourTests()
    {
        // Create next behaviour (mock)
        _mockNextBehaviour = Substitute.For<RequestHandlerDelegate<ErrorOr<Gym>>>();

        // Create validator (mock)
        _mockValidator = Substitute.For<IValidator<CreateGymCommand>>();

        // Create ValidationBehaviour (SUT)
        _validationBehaviour = new ValidationBehaviour<CreateGymCommand, ErrorOr<Gym>>(_mockValidator);

    }

    [Fact]
    public async Task InvokeBehaviour_WhenValidatorResultIsValid_ShouldInvokeNextBehaviour()
    {
        // Arrange

        // Create request
        var createGymRequest = GymCommandFactory.CreateCreateGymCommand();

        var gym = GymFactory.CreateGym();
        _mockNextBehaviour.Invoke().Returns(gym);

        _mockValidator
            .ValidateAsync(createGymRequest, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());


        // Act
        // Invoke behaviour 
        var result = await _validationBehaviour.Handle(createGymRequest, _mockNextBehaviour, default);

        // Assert
        // Result from invoking behaviour was the result returned by the next behaviour
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(gym);
    }

    [Fact]
    public async Task InvokeBehaviour_WhenValidatorResultIsInValid_ShouldReturnListOfValidationErrors()
    {
        // Arrange

        // Create request
        var createGymRequest = GymCommandFactory.CreateCreateGymCommand();

        // Mock validation failures
        List<ValidationFailure> validationFailures = [
            new(propertyName: "Foo", errorMessage: "bad foo")
        ];

        _mockValidator
            .ValidateAsync(createGymRequest, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(validationFailures));


        // Act
        // Invoke behaviour 
        var result = await _validationBehaviour.Handle(createGymRequest, _mockNextBehaviour, default);

        // Assert
        // Result from invoking behaviour was the result returned by the next behaviour
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Foo");
        result.FirstError.Description.Should().Be("bad foo");
    }
}
