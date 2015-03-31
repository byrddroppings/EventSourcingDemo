using System;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using MediatorLib;
using Dapper;
using EventSourcingDemo.Models;
using Newtonsoft.Json;

namespace EventSourcingDemo.Features
{
    public class ApproveLogCommand
    {
        public int CommandLogId { get; set; }
    }

    public class ApproveCommand<T>
    {
        public ApproveCommand(T command)
        {
            Command = command;
        }

        public T Command { get; private set; }
    }

    public class ValidateApproveLogCommand : IValidateHandler<ApproveLogCommand>
    {
        private readonly IRepository _repository;

        public ValidateApproveLogCommand(IRepository repository)
        {
            _repository = repository;
        }

        public void Validate(ApproveLogCommand command)
        {
            var sql = "select CommandLogId, EntityId from CommandLog where CommandLogId = @CommandLogId";
            var commandLog = _repository.Connection.Query<CommandLog>(sql, command).SingleOrDefault();

            sql = "select count(*) from CommandLog where EntityId = @EntityId and CommandLogId < @CommandLogId and Status = 'Pending'";
            var count = _repository.Connection.Query<long>(sql, commandLog).SingleOrDefault();

            if (count > 0)
                throw new InvalidOperationException("This command cannot be approved until all prior pending commands are approved.");
        }
    }

    public class ApproveCommandLogHandler : ICommandHandler<ApproveLogCommand>
    {
        private readonly IMediator _mediator;
        private readonly IRepository _repository;

        public ApproveCommandLogHandler(IMediator mediator, IRepository repository)
        {
            _mediator = mediator;
            _repository = repository;
        }

        public void Execute(ApproveLogCommand command)
        {
            var commandLog = _repository.Connection.Query<CommandLog>("select * from CommandLog where CommandLogId = @CommandLogId", command).FirstOrDefault();

            if (commandLog == null)
                throw new InvalidOperationException("Can't find CommandLogId " + command.CommandLogId);

            var serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Full
            };

            var originalCommand = JsonConvert.DeserializeObject(commandLog.Data, serializerSettings);
            var approveCommandType = typeof (ApproveCommand<>).MakeGenericType(originalCommand.GetType());
            var approveCommand = Activator.CreateInstance(approveCommandType, originalCommand);

            var execute = _mediator.GetType().GetMethod("TryExecute").MakeGenericMethod(approveCommand.GetType());
            execute.Invoke(_mediator, new []{approveCommand});

            _repository.Connection.Execute("update CommandLog set Status = 'Approved' where CommandLogId = @CommandLogId", command);
        }
    }
}