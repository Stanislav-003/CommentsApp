using backend.Abstractions.Messaging;
using backend.Shared.Errors;
using FluentValidation;

namespace backend.Decorators;

public class ValidationDecorator
{
    public class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> innerHandler,
        IEnumerable<IValidator<TCommand>> validators)
        : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        public async Task Handle(TCommand command, CancellationToken cancellationToken)
        {
            var errors = validators
                .Select(v => v.Validate(command))
                .SelectMany(r => r.Errors)
                .Where(e => e != null)
                .Select(e => new Error(e.ErrorMessage))
                .ToList();

            if (errors.Any())
                throw new CustomValidationException("Validation failed", errors);

            await innerHandler.Handle(command, cancellationToken);
        }
    }

    public class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler,
        IEnumerable<IValidator<TCommand>> validators)
        : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public async Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken)
        {
            var errors = validators
                .Select(v => v.Validate(command))
                .SelectMany(r => r.Errors)
                .Where(e => e != null)
                .Select(e => new Error(e.ErrorMessage))
                .ToList();

            if (errors.Any())
                throw new CustomValidationException("Validation failed", errors);

            return await innerHandler.Handle(command, cancellationToken);
        }
    }

    public class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> innerHandler,
        IEnumerable<IValidator<TQuery>> validators)
        : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        public async Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken)
        {
            var errors = validators
                .Select(v => v.Validate(query))
                .SelectMany(r => r.Errors)
                .Where(e => e != null)
                .Select(e => new Error(e.ErrorMessage))
                .ToList();

            if (errors.Any())
                throw new CustomValidationException("Validation failed", errors);

            return await innerHandler.Handle(query, cancellationToken);
        }
    }
}
