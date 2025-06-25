using GymManagement.Application.Common.Interfaces;
using GymManagement.Infrastructure.Common.Persistence;
using GymManagement.Infrastructure.Subscriptions.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GymManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Register infrastructure services here
        services.AddDbContext<GymManagementDbContext>(options =>
            options.UseSqlite("Data Source=GymManagement.db"));
        services.AddScoped<ISubscriptionRepository, SubscriptionsRepository>();
        
        return services;
    }
}
