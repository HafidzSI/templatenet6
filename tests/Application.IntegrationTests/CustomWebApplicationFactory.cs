using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using NetCa.Application.Common.Interfaces;
using NetCa.Application.Common.Models;
using NetCa.Infrastructure.Persistence;
using NetCa.Infrastructure.Persistence.Interceptors;

namespace NetCa.Application.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly AppSetting _appSetting;

    private static Mock<IUserAuthorizationService> AuthMock { get; set; }

    private static Mock<IProducerService> ProduceMock { get; set; }

    private static Mock<IRedisService> RedisMock { get; set; }

    public CustomWebApplicationFactory(AppSetting appSetting)
    {
        _appSetting = appSetting;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices((_, services) =>
        {
            services.RemoveAll<IWebHostEnvironment>().AddSingleton(Mock.Of<IWebHostEnvironment>(x =>
                x.EnvironmentName == Constants.EnvironmentNameTest && x.ApplicationName == Constants.ApplicationName));

            services.AddLogging();

            AuthMock = new Mock<IUserAuthorizationService>();

            services.RemoveAll<IUserAuthorizationService>().AddTransient(_ =>
            {
                AuthMock.Setup(x => x.GetAuthorizedUser()).Returns(MockData.GetAuthorizedUser());
                AuthMock.Setup(x => x.GetUserNameSystem()).Returns(Constants.SystemName);

                return AuthMock.Object;
            });

            ProduceMock = new Mock<IProducerService>();

            services.RemoveAll<IProducerService>().AddSingleton(_ => ProduceMock.Setup(x =>
                x.SendAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true));

            RedisMock = new Mock<IRedisService>();

            services.RemoveAll<IRedisService>().AddSingleton(_ =>
            {
                RedisMock.Setup(x =>
                    x.SaveSetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>(), false));
                RedisMock.Setup(x =>
                        x.SaveSubAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), false))
                    .ReturnsAsync("KEYS");

                return RedisMock.Object;
            });

            services.RemoveAll<AuditableEntitySaveChangesInterceptor>()
                .AddScoped<AuditableEntitySaveChangesInterceptor>();

            services.RemoveAll<ApplicationDbContext>().AddEntityFrameworkNpgsql()
                .AddDbContext<ApplicationDbContext>(
                    (provider, options) =>
                        options
                            .UseInternalServiceProvider(provider)
                            .UseNpgsql(
                                _appSetting.ConnectionStrings.DefaultConnection,
                                x =>
                                {
                                    x.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                                    x.CommandTimeout(_appSetting.DatabaseSettings.CommandTimeout);
                                    x.EnableRetryOnFailure(
                                        _appSetting.DatabaseSettings.MaxRetryCount,
                                        TimeSpan.FromSeconds(_appSetting.DatabaseSettings.MaxRetryDelay),
                                        null);
                                }));
        });
    }
}