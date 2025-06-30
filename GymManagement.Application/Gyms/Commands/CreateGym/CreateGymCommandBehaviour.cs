using ErrorOr;
using GymManagement.Domain.Gyms;
using MediatR;

namespace GymManagement.Application.Gyms.Commands.CreateGym;

public class CreateGymCommandBehaviour : IPipelineBehavior<CreateGymCommand, ErrorOr<Gym>>
{
    public async Task<ErrorOr<Gym>> Handle(CreateGymCommand command, RequestHandlerDelegate<ErrorOr<Gym>> next, CancellationToken cancellationToken)
    {
        // Validate 
        var vallidator = new CreateGymCommandValidator();
        var validationResult = await vallidator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.Errors
                .Select(error => Error.Validation(
                    code: error.PropertyName,
                    description: error.ErrorMessage))
                .ToList();
        }

        return await next();
    }
}