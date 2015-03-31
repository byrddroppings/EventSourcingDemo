using System;
using MediatorLib;
using Dapper;
using EventSourcingDemo.Models;

namespace EventSourcingDemo.Features
{
    public class CreatePersonCommand
    {
        public Person Person { get; set; }
        public string Status { get; set; }
    }

    public class CreatePersonCommandHandler : PersonCommandHandler, ICommandHandler<CreatePersonCommand>
    {
        public CreatePersonCommandHandler(IMediator mediator, IRepository repository) : base(mediator, repository)
        {
        }

        public void Execute(CreatePersonCommand command)
        {
            var person = command.Person;
            person.PersonId = Guid.NewGuid();
            person.Status = command.Status;

            AddToCommandLog(person.PersonId, command.Status, command);
            CreatePerson(person);
        }

        private void CreatePerson(Person person)
        {
            const string sql = @"insert into Person(PersonId,FirstName,LastName)
                                 values(@PersonId,@FirstName,@LastName)";

            Repository.Connection.Execute(sql, person);
        }
    }

    public class RejectCreatePersonCommandHandler : PersonCommandHandler, ICommandHandler<RejectCommand<CreatePersonCommand>>
    {
        public RejectCreatePersonCommandHandler(IMediator mediator, IRepository repository) : base(mediator, repository)
        {
        }

        public void Execute(RejectCommand<CreatePersonCommand> command)
        {
            Repository.Connection.Execute("delete from Person where PersonId = @PersonId", command.Command.Person);
        }
    }

}