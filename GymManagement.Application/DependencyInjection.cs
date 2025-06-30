using ErrorOr;
using GymManagement.Application.Gyms.Commands.CreateGym;
using GymManagement.Domain.Gyms;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace GymManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection));
            cfg.AddBehavior<
                IPipelineBehavior<CreateGymCommand, ErrorOr<Gym>>,
                CreateGymCommandBehaviour
            >();
        });
        return services;
    }
}
