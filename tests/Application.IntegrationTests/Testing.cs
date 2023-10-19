// -----------------------------------------------------------------------------------
// Testing.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NetCa.Application.Common.Interfaces;
using NetCa.Application.Common.Models;
using NetCa.Infrastructure.Persistence;
using Npgsql;
using NUnit.Framework;
using Respawn;

namespace NetCa.Application.IntegrationTests
{
    /// <summary>
    /// Testing
    /// </summary>
    [SetUpFixture]
    public class Testing : ITestDatabase
    {
        private static ITestDatabase _database;

        private static WebApplicationFactory<Program> _factory = null!;

        private static Mock<IUserAuthorizationService> AuthMock => null;

        private string _connectionString = null!;
        private DbConnection _connection = null!;
        private Respawner _respawner = null!;

        public static IServiceScopeFactory ScopeFactory { get; set; }

        [OneTimeSetUp]
        public async Task RunBeforeAnyTests()
        {
            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.Test.json", true, true)
                    .AddJsonFile($"appsettings.Test.Local.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                var appSetting = configuration.Get<AppSetting>();

                _factory = new CustomWebApplicationFactory(appSetting);

                ScopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();

                _database = await TestDatabaseFactory.CreateAsync(appSetting);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString(), e);
            }
        }

        public async Task InitialiseAsync(AppSetting appSetting)
        {
            _connectionString = appSetting.ConnectionStrings.DefaultConnection;

            _connection = new NpgsqlConnection(_connectionString);

            await _connection.OpenAsync();

            using var scope = ScopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await context.Database.MigrateAsync();

            _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
            {
                TablesToIgnore = new Respawn.Graph.Table[] { "__EFMigrationsHistory" },
                DbAdapter = DbAdapter.Postgres
            });
        }

        public static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
        {
            using var scope = ScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetService<IMediator>();

            return await mediator.Send(request);
        }

        public static async Task AddAsync<TEntity>(TEntity entity)
            where TEntity : class
        {
            using var scope = ScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            context.Add(entity);

            await context.SaveChangesAsync();
        }

        public static async Task<TEntity> FindAsync<TEntity>(params object[] keyValues)
            where TEntity : class
        {
            using var scope = ScopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return await context.FindAsync<TEntity>(keyValues);
        }

        public static async Task<int> CountAsync<TEntity>()
            where TEntity : class
        {
            using var scope = ScopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            return await context.Set<TEntity>().CountAsync();
        }

        #region MockingData

        public static void MockUserId(Guid userId)
        {
            using var scope = ScopeFactory.CreateScope();
            var userAuthorizationService = scope.ServiceProvider.GetService<IUserAuthorizationService>();

            var mock = userAuthorizationService.GetAuthorizedUser();
            mock.UserId = userId;

            AuthMock.Setup(x => x.GetAuthorizedUser()).Returns(mock);
        }

        public static void MockEmail(string email = Constants.SystemEmail)
        {
            var mock = MockData.GetUserEmailInfo();
            mock.Email = email;

            AuthMock.Setup(x => x.GetEmailByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mock);
        }

        public static void MockUserCustomerCode(string customerCode = "*")
        {
            using var scope = ScopeFactory.CreateScope();
            var userAuthorizationService = scope.ServiceProvider.GetService<IUserAuthorizationService>();

            var mock = userAuthorizationService.GetAuthorizedUser();
            mock.CustomerCode = customerCode;

            AuthMock.Setup(x => x.GetAuthorizedUser()).Returns(mock);
        }

        public static void MockAttribute(
            List<string> customerCode = null,
            List<string> plantCode = null,
            List<string> customerSiteCode = null,
            List<string> abc = null)
        {
            customerCode ??= new List<string> { "*" };
            plantCode ??= new List<string> { "*" };
            customerSiteCode ??= new List<string> { "*" };
            abc ??= new List<string> { "*" };

            var dict = new Dictionary<string, List<string>>
            {
                { Constants.CustomerName, customerCode },
                { Constants.PlantFieldName, plantCode },
                { Constants.CustomerSiteFieldName, customerSiteCode },
                { Constants.ABCFieldName, abc }
            };

            AuthMock.Setup(x => x.GetUserAttributesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(dict);
        }

        #endregion

        public DbConnection GetConnection()
        {
            return _connection;
        }

        public async Task ResetAsync()
        {
            await _respawner.ResetAsync(_connection);
        }

        public async Task DisposeAsync()
        {
            await _connection.DisposeAsync();
        }

        public static async Task ResetData()
        {
            try
            {
                await _database.ResetAsync();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        [OneTimeTearDown]
        public static async Task RunAfterAnyTests()
        {
            await ResetData();
            await _database.DisposeAsync();
            await _factory.DisposeAsync();
        }
    }
}