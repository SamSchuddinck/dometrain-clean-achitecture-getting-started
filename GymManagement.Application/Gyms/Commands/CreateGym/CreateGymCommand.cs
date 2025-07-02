using System.Security;
using ErrorOr;
using GymManagement.Application.Authorization;
using GymManagement.Domain.Gyms;
using MediatR;
using Microsoft.AspNetCore.Authorization.Infrastructure;


namespace GymManagement.Application.Gyms.Commands.CreateGym;

[Authorize(Roles = "Admin")]
public record CreateGymCommand(string Name, Guid SubscriptionId) : IRequest<ErrorOr<Gym>>;