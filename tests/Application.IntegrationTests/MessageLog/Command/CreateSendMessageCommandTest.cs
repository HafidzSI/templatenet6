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
/// CreateSendMessageCommandTest
/// </summary>
public class CreateSendMessageCommandTest : BaseTestFixture
{
    [Test]
    public async Task ShouldCreateSendMessage()
    {
        using var scope = ScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var id = Guid.NewGuid();

        var query = new CreateSendMessageCommand
        {
            MessageBroker = new MessageBroker { Id = id }
        };

        await SendAsync(query);

        var test = await context.MessageBroker
            .FirstOrDefaultAsync(x => x.Id.Equals(id));

        test.Should().NotBeNull();
    }
}