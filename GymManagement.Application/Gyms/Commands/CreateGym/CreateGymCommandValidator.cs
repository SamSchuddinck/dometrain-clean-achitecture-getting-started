using FluentValidation;

namespace GymManagement.Application.Gyms.Commands.CreateGym;
public class CreateGymCommandValidator : AbstractValidator<CreateGymCommand>
{
    public CreateGymCommandValidator()
    {
        RuleFor(command => command.Name)
            .MinimumLength(3)
            .MaximumLength(100);
            
    }
}