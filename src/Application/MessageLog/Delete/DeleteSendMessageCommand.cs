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
/// DeleteSendMessageCommand
/// </summary>
public class DeleteSendMessageCommand : IRequest<bool>
{
    /// <summary>
    /// Handling DeleteSendMessageCommand
    /// </summary>
    public class DeleteSendMessageCommandHandler : IRequestHandler<DeleteSendMessageCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger _logger;
        private readonly AppSetting _appSetting;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteSendMessageCommandHandler"/> class.
        /// </summary>
        /// <param name="context">Set context to perform CRUD into Database</param>
        /// <param name="logger">Set logger to perform logging</param>
        /// <param name="appSetting">Set dateTime to get Application Setting</param>
        public DeleteSendMessageCommandHandler(
            IApplicationDbContext context, ILogger<DeleteSendMessageCommandHandler> logger, AppSetting appSetting)
        {
            _context = context;
            _logger = logger;
            _appSetting = appSetting;
        }

        /// <summary>
        /// Handle Delete Send Message
        /// </summary>
        /// <param name="request">
        /// The encapsulated request body
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token to perform cancel the operation
        /// </param>
        /// <returns></returns>
        public async Task<bool> Handle(DeleteSendMessageCommand request, CancellationToken cancellationToken)
        {
            bool status = false;

            try
            {
                _logger.LogDebug("Delete send message process");

                var lifeTime = _appSetting.DataLifetime.Changelog;

                var date = DateTime.Now.AddDays(-lifeTime);

                await _context.MessageBroker
                    .Where(x => date > x.StoredDate)
                    .DeleteAsync(cancellationToken);

                status = true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to delete received message: {Message}", e.Message);
            }

            _logger.LogDebug("Delete send message done");

            return status;
        }
    }
}