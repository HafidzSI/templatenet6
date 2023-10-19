using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NetCa.Application.Common.Interfaces;
using NetCa.Domain.Entities;

namespace NetCa.Application.MessageLog.Create;

/// <summary>
/// CreateSendMessageCommand
/// </summary>
public class CreateSendMessageCommand : IRequest<Unit>
{
    /// <summary>
    /// Gets or sets MessageBroker
    /// </summary>
    public MessageBroker MessageBroker { get; set; }

    /// <summary>
    /// Handling CreateSendMessageCommand
    /// </summary>
    public class CreateSendMessageCommandHandler : IRequestHandler<CreateSendMessageCommand, Unit>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateSendMessageCommandHandler"/> class.
        /// </summary>
        /// <param name="context">Set context to perform CRUD into Database</param>
        /// <param name="logger">Set logger to perform logging</param>
        public CreateSendMessageCommandHandler(
            IApplicationDbContext context,
            ILogger<CreateSendMessageCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Handle Create Send Message Command
        /// </summary>
        /// <param name="request">
        /// The encapsulated request body
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token to perform cancel the operation
        /// </param>
        /// <returns></returns>
        public async Task<Unit> Handle(CreateSendMessageCommand request, CancellationToken cancellationToken)
        {
            _context.MessageBroker.Add(request.MessageBroker);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogDebug("Save send message success");

            return Unit.Value;
        }
    }
}