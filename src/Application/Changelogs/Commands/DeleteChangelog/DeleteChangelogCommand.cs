using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NetCa.Application.Common.Interfaces;
using NetCa.Application.Common.Models;
using Z.EntityFramework.Plus;

namespace NetCa.Application.Changelogs.Commands.DeleteChangelog;

/// <summary>
/// DeleteChangelogCommand
/// </summary>
public class DeleteChangelogCommand : IRequest<bool>
{
    /// <summary>
    /// Handling DeleteChangelogCommand
    /// </summary>
    public class DeleteChangelogCommandHandler : IRequestHandler<DeleteChangelogCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger _logger;
        private readonly AppSetting _appSetting;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteChangelogCommandHandler"/> class.
        /// </summary>
        /// <param name="context">Set context to perform CRUD into Database</param>
        /// <param name="logger">Set logger to perform logging</param>
        /// <param name="appSetting">Set dateTime to get Application Setting</param>
        /// <returns></returns>
        public DeleteChangelogCommandHandler(
            IApplicationDbContext context, ILogger<DeleteChangelogCommandHandler> logger, AppSetting appSetting)
        {
            _context = context;
            _logger = logger;
            _appSetting = appSetting;
        }

        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="request">
        /// The encapsulated request body
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token to perform cancel the operation
        /// </param>
        /// <returns>A bool true or false</returns>
        public async Task<bool> Handle(DeleteChangelogCommand request, CancellationToken cancellationToken)
        {
            bool status = false;

            try
            {
                int lifeTime = _appSetting.DataLifetime.Changelog;

                var date = DateTime.Now.AddDays(-lifeTime);

                await _context.Changelogs
                    .Where(x => date > x.ChangeDate)
                    .DeleteAsync(cancellationToken);

                status = true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to delete changelog: {Message}", e.Message);
            }

            return status;
        }
    }
}