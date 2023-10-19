// -----------------------------------------------------------------------------------
// DeleteChangelogJob.cs 2023
// Copyright DAD RnD. All rights reserved.
// DAD Helpdesk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCa.Application.Changelogs.Commands.DeleteChangelog;
using NetCa.Application.MessageLog.Delete;
using Quartz;

namespace NetCa.Infrastructure.Jobs;

/// <summary>
/// DeleteChangelogJob
/// </summary>
public class DeleteChangelogJob : BaseJob<DeleteChangelogJob>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteChangelogJob"/> class.
    /// </summary>
    /// <param name="serviceScopeFactory"></param>
    /// <param name="logger"></param>
    public DeleteChangelogJob(IServiceScopeFactory serviceScopeFactory, ILogger<DeleteChangelogJob> logger)
        : base(logger, serviceScopeFactory)
    {
    }

    /// <summary>
    /// Execute
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task Execute(IJobExecutionContext context)
    {
        try
        {
            Logger.LogDebug("Process delete changelog");

            using var scope = ServiceScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Send(new DeleteChangelogCommand());
            await mediator.Send(new DeleteReceivedMessageCommand());
            await mediator.Send(new DeleteSendMessageCommand());
        }
        catch (Exception e)
        {
            Logger.LogError("Error when running worker delete changelog: {Message}", e.Message);
        }
        finally
        {
            Logger.LogDebug("Delete changelog done");
        }
    }
}