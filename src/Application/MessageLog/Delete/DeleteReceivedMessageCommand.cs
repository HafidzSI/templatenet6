using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NetCa.Application.Common.Interfaces;
using NetCa.Application.Common.Models;
using Z.EntityFramework.Plus;

namespace NetCa.Application.MessageLog.Delete;

/// <summary>
/// DeleteReceivedMessageCommand
/// </summary>
public class DeleteReceivedMessageCommand : IRequest<bool>
{
    /// <summary>
    /// Handling DeleteReceivedMessageCommand
    /// </summary>
    public class DeleteReceivedMessageCommandHandler : IRequestHandler<DeleteReceivedMessageCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger _logger;
        private readonly AppSetting _appSetting;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteReceivedMessageCommandHandler"/> class.
        /// </summary>
        /// <param name="context">Set context to perform CRUD into Database</param>
        /// <param name="logger">Set logger to perform logging</param>
        /// <param name="appSetting">Set dateTime to get Application Setting</param>
        public DeleteReceivedMessageCommandHandler(
            IApplicationDbContext context, ILogger<DeleteReceivedMessageCommandHandler> logger, AppSetting appSetting)
        {
            _context = context;
            _logger = logger;
            _appSetting = appSetting;
        }

        /// <summary>
        /// Handle Delete Received Message
        /// </summary>
        /// <param name="request">
        /// The encapsulated request body
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token to perform cancel the operation
        /// </param>
        /// <returns></returns>
        public async Task<bool> Handle(DeleteReceivedMessageCommand request, CancellationToken cancellationToken)
        {
            var status = false;

            try
            {
                _logger.LogDebug("Delete received message process");

                var lifeTime = _appSetting.DataLifetime.Changelog;

                var date = DateTime.Now.AddDays(-lifeTime);

                await _context.ReceivedMessageBroker
                    .Where(x => date > x.TimeIn)
                    .DeleteAsync(cancellationToken);

                status = true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to delete received message: {Message}", e.Message);
            }

            _logger.LogDebug("Delete received message done");

            return status;
        }
    }
}