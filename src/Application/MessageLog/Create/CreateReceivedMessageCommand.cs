using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NetCa.Application.Common.Interfaces;
using NetCa.Domain.Entities;

namespace NetCa.Application.MessageLog.Create;

/// <summary>
/// CreateReceivedMessageCommand
/// </summary>
public class CreateReceivedMessageCommand : IRequest<Unit>
{
    /// <summary>
    /// Gets or sets Message
    /// </summary>
    public ReceivedMessageBroker Message { get; set; }

    /// <summary>
    /// Handling CreateReceivedMessageCommand
    /// </summary>
    public class CreateReceivedMessageCommandHandler : IRequestHandler<CreateReceivedMessageCommand, Unit>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateReceivedMessageCommandHandler"/> class.
        /// </summary>
        /// <param name="context">Set context to perform CRUD into Database</param>
        /// <param name="logger">Set logger to perform logging</param>
        public CreateReceivedMessageCommandHandler(
            IApplicationDbContext context,
            ILogger<CreateReceivedMessageCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Handle Create Received Message Command
        /// </summary>
        /// <param name="request">
        /// The encapsulated request body
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token to perform cancel the operation
        /// </param>
        /// <returns></returns>
        public async Task<Unit> Handle(CreateReceivedMessageCommand request, CancellationToken cancellationToken)
        {
            _context.ReceivedMessageBroker.Add(request.Message);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogDebug("Save received message success");

            return Unit.Value;
        }
    }
}