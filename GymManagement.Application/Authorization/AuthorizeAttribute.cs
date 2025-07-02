using System;

namespace GymManagement.Application.Authorization;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class AuthorizeAttribute: Attribute
{
    public string? Permissions { get; set; }
}
