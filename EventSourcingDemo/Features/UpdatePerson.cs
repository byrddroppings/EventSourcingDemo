using MediatorLib;
using Dapper;
using EventSourcingDemo.Models;

namespace EventSourcingDemo.Features
{
    public class UpdatePersonCommand
    {
        public Rollback<Person> Rollback { get; set; }
        public string Status { get; set; }
    }

    public class UpdatePersonCommandHandler : PersonCommandHandler, ICommandHandler<UpdatePersonCommand>
    {
        public UpdatePersonCommandHandler(IMediator mediator, IRepository repository)
            : base(mediator, repository)
        {
        }

        public void Execute(UpdatePersonCommand command)
        {
            command.Rollback.After.Status = command.Status;
            AddToCommandLog(command.Rollback.Before.PersonId, command.Status, command);
            UpdatePerson(command.Rollback.After);

            string sql = "insert into PersonLog(PersonId,FirstName,LastName) values(@PersonId,@FirstName,@LastName)";
            Repository.Connection.Execute(sql, command.Rollback.After);
        }
    }

    public class RejectUpdatePersonCommandHandler : PersonCommandHandler, ICommandHandler<RejectCommand<UpdatePersonCommand>>
    {
        public RejectUpdatePersonCommandHandler(IMediator mediator, IRepository repository) : base(mediator, repository)
        {
        }

        public void Execute(RejectCommand<UpdatePersonCommand> command)
        {
            const string sql =
                "update Person set FirstName = @FirstName, LastName = @LastName where PersonId = @PersonId";
            var before = command.Command.Rollback.Before;
            Repository.Connection.Execute(sql, before);
        }
    }
}