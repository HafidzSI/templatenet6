using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetCa.Application.MessageLog.Delete;
using NetCa.Domain.Entities;
using NetCa.Infrastructure.Persistence;
using NUnit.Framework;
using static NetCa.Application.IntegrationTests.Testing;

namespace NetCa.Application.IntegrationTests.MessageLog.Command;

/// <summary>
/// DeleteReceivedMessageCommandTest
/// </summary>
public class DeleteReceivedMessageCommandTest : BaseTestFixture
{
    [Test]
    [TestCase("2000-01-01", true)]
    [TestCase("2100-01-01", false)]
    public async Task ShouldDeleteReceivedMessage(DateTime changeDate, bool shouldDelete)
    {
        using var scope = ScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var query = new DeleteReceivedMessageCommand();

        var id = Guid.NewGuid();

        context.ReceivedMessageBroker.Add(new ReceivedMessageBroker
        {
            Id = id,
            TimeIn = changeDate
        });

        await context.SaveChangesAsync();

        var test = await context.ReceivedMessageBroker
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id.Equals(id));

        test.Should().NotBeNull();

        (await SendAsync(query)).Should().BeTrue();

        test = await context.ReceivedMessageBroker
            .FirstOrDefaultAsync(x => x.Id.Equals(id));

        if (shouldDelete)
            test.Should().BeNull();
        else
            test.Should().NotBeNull();
    }
}