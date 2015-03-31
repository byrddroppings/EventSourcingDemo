using System;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using MediatorLib;
using Dapper;
using EventSourcingDemo.Models;
using Newtonsoft.Json;

namespace EventSourcingDemo.Features
{
    public class RejectLogCommand
    {
        public int CommandLogId { get; set; }
    }

    public class RejectCommand<T>
    {
        public RejectCommand(T command)
        {
            Command = command;
        }

        public T Command { get; private set; }
    }

    public class ValidateRejectLogCommand : IValidateHandler<RejectLogCommand>
    {
        private readonly IRepository _repository;

        public ValidateRejectLogCommand(IRepository repository)
        {
            _repository = repository;
        }

        public void Validate(RejectLogCommand command)
        {
            var sql = "select CommandLogId, EntityId from CommandLog where CommandLogId = @CommandLogId";
            var commandLog = _repository.Connection.Query<CommandLog>(sql, command).SingleOrDefault();

            sql = "select count(*) from CommandLog where EntityId = @EntityId and CommandLogId > @CommandLogId and Status = 'Pending'";
            var count = _repository.Connection.Query<long>(sql, commandLog).SingleOrDefault();

            if (count > 0)
                throw new InvalidOperationException("This command cannot be rejected until all subsequent pending commands are rejected.");
        }
    }

    public class RejectCommandLogHandler : ICommandHandler<RejectLogCommand>
    {
        private readonly IMediator _mediator;
        private readonly IRepository _repository;

        public RejectCommandLogHandler(IMediator mediator, IRepository repository)
        {
            _mediator = mediator;
            _repository = repository;
        }

        public void Execute(RejectLogCommand command)
        {
            var commandLog =
                _repository.Connection.Query<CommandLog>("select * from CommandLog where CommandLogId = @CommandLogId",
                    command).FirstOrDefault();

            if (commandLog == null)
                throw new InvalidOperationException("Can't find CommandLogId " + command.CommandLogId);

            var serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Full
            };

            var originalCommand = JsonConvert.DeserializeObject(commandLog.Data, serializerSettings);
            var rejectCommandType = typeof (RejectCommand<>).MakeGenericType(originalCommand.GetType());
            var rejectCommand = Activator.CreateInstance(rejectCommandType, originalCommand);

            var execute = _mediator.GetType().GetMethod("TryExecute").MakeGenericMethod(rejectCommand.GetType());
            execute.Invoke(_mediator, new[] {rejectCommand});

            _repository.Connection.Execute(
                "update CommandLog set Status = 'Rejected' where CommandLogId = @CommandLogId", command);
        }
    }
}