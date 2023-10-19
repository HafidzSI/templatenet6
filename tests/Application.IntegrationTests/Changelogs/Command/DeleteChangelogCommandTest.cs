using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetCa.Application.Changelogs.Commands.DeleteChangelog;
using NetCa.Domain.Entities;
using NetCa.Infrastructure.Persistence;
using NUnit.Framework;
using static NetCa.Application.IntegrationTests.Testing;

namespace NetCa.Application.IntegrationTests.Changelogs.Command;

/// <summary>
/// DeleteChangelogCommandTest
/// </summary>
public class DeleteChangelogCommandTest : BaseTestFixture
{
    [Test]
    [TestCase("2000-01-01", true)]
    [TestCase("2100-01-01", false)]
    public async Task ShouldDeleteChangelog(DateTime changeDate, bool shouldDelete)
    {
        using var scope = ScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var query = new DeleteChangelogCommand();

        var id = Guid.NewGuid();

        context.Changelogs.Add(new Changelog
        {
            Id = id,
            ChangeDate = changeDate
        });

        await context.SaveChangesAsync();

        var test = await context.Changelogs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id.Equals(id));

        test.Should().NotBeNull();

        (await SendAsync(query)).Should().BeTrue();

        test = await context.Changelogs
            .FirstOrDefaultAsync(x => x.Id.Equals(id));

        if (shouldDelete)
            test.Should().BeNull();
        else
            test.Should().NotBeNull();
    }
}