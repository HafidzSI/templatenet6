// -----------------------------------------------------------------------------------
// DepedencyInjection.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using NetCa.Application.Common.Behaviors;
using NetCa.Application.Common.Interfaces;
using Scrutor;

namespace NetCa.Application;

/// <summary>
/// DepedencyInjection
/// </summary>
public static class DepedencyInjection
{
    /// <summary>
    /// AddApplicationServices
    /// </summary>
    /// <param name="services"></param>
    public static void AddApplicationServices(this IServiceCollection services)
    {
        var assembly = typeof(DepedencyInjection).Assembly;

        services.AddAutoMapper(assembly);
        services.AddValidatorsFromAssembly(assembly);
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenRequestPreProcessor(typeof(LoggingBehavior<>));
            cfg.AddOpenBehavior(typeof(RateLimitPolicyBehavior<,>), ServiceLifetime.Singleton);
            cfg.AddOpenBehavior(typeof(BulkheadPolicyBehavior<,>), ServiceLifetime.Singleton);
            cfg.AddOpenBehavior(typeof(CircuitBreakerPolicyBehavior<,>), ServiceLifetime.Singleton);
            cfg.AddOpenBehavior(typeof(TimeoutPolicyBehavior<,>));
            cfg.AddOpenBehavior(typeof(UnhandledExceptionBehavior<,>));
            cfg.AddOpenBehavior(typeof(RequestBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(PerformanceBehavior<,>));
            cfg.AddOpenBehavior(typeof(CachingBehavior<,>));
            cfg.AddOpenBehavior(typeof(FallbackBehavior<,>));
            cfg.AddOpenBehavior(typeof(RetryPolicyBehavior<,>));
        });

        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(ICachePolicy<,>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(IFallbackHandler<,>)))
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithTransientLifetime());
    }
}