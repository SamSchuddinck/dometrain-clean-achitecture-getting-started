using System.Security;
using ErrorOr;
using GymManagement.Application.Authorization;
using GymManagement.Domain.Gyms;
using MediatR;


namespace GymManagement.Application.Gyms.Commands.CreateGym;

[Authorize(Permissions = "gyms:create")]
public record CreateGymCommand(string Name, Guid SubscriptionId) : IRequest<ErrorOr<Gym>>;