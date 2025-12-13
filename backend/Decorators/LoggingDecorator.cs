using backend.Abstractions.Messaging;

namespace backend.Behaviors;

public class LoggingDecorator
{
    public class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> innerHandler,
        ILogger<CommandBaseHandler<TCommand>> logger)
        : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        public async Task Handle(TCommand command, CancellationToken cancellationToken)
        {
            string requestName = typeof(TCommand).Name;

            logger.LogInformation($"Processing request {requestName}");

            await innerHandler.Handle(command, cancellationToken);

            logger.LogInformation($"Completed request {requestName}");
        }
    }

    public class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler,
        ILogger<CommandHandler<TCommand, TResponse>> logger)
        : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public async Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken)
        {
            string requestName = typeof(TCommand).Name;

            logger.LogInformation($"Processing request {requestName}");

            TResponse result = await innerHandler.Handle(command, cancellationToken);

            logger.LogInformation($"Completed request {requestName}");

            return result;
        }
    }

    public class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> innerHandler,
        ILogger<QueryHandler<TQuery, TResponse>> logger)
        : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        public async Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken)
        {
            string requestName = typeof(TQuery).Name;

            logger.LogInformation($"Processing request {requestName}");

            TResponse result = await innerHandler.Handle(query, cancellationToken);

            logger.LogInformation($"Completed request {requestName}");

            return result;
        }
    }
}
