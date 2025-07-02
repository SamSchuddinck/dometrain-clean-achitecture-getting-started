using System;
using System.Reflection;
using ErrorOr;
using GymManagement.Application.Authorization;
using GymManagement.Application.Common.Interfaces;
using MediatR;

namespace GymManagement.Application.Common.Behaviours;

public class AuthorizationBehaviour<TRequest, TResponse>(ICurrentUserProvider _currentUserProvider)
    : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : IErrorOr
{
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var authorizationAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>().ToList();

        if (authorizationAttributes.Count == 0)
        {
            return next();
        }

        var requiredPermissions = authorizationAttributes.SelectMany(authorizationAttribute => authorizationAttribute.Permissions?.Split(',') ?? []).ToList();
        var requiredRoles = authorizationAttributes.SelectMany(authorizationAttribute => authorizationAttribute.Roles?.Split(',') ?? []).ToList();

        var currentUser = _currentUserProvider.GetCurrentUser();

        if (requiredRoles.Except(currentUser.Roles).Any())
        {
            return (dynamic)Error.Unauthorized("User is forbidden from taking this action");
        }

        return next();

    }
}
