using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetCa.Application.MessageLog.Create;
using NetCa.Domain.Entities;
using NetCa.Infrastructure.Persistence;
using NUnit.Framework;
using static NetCa.Application.IntegrationTests.Testing;

namespace NetCa.Application.IntegrationTests.MessageLog.Command;

/// <summary>
/// CreateReceivedMessageCommandTest
/// </summary>
public class CreateReceivedMessageCommandTest : BaseTestFixture
{
    [Test]
    public async Task ShouldCreateReceivedMessage()
    {
        using var scope = ScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var id = Guid.NewGuid();

        var query = new CreateReceivedMessageCommand
        {
            Message = new ReceivedMessageBroker { Id = id }
        };

        await SendAsync(query);

        var test = await context.ReceivedMessageBroker
            .FirstOrDefaultAsync(x => x.Id.Equals(id));

        test.Should().NotBeNull();
    }
}