using System;
using MediatorLib;
using Dapper;
using EventSourcingDemo.Models;

namespace EventSourcingDemo.Features
{
    public abstract class PersonCommandHandler
    {
        protected readonly IMediator Mediator;
        protected readonly IRepository Repository;

        protected PersonCommandHandler(IMediator mediator, IRepository repository)
        {
            Mediator = mediator;
            Repository = repository;
        }

        protected void UpdatePerson(Person person)
        {
            const string sql = @"update Person
                                 set FirstName = @FirstName, LastName = @LastName
                                 where PersonId = @PersonId";

            Repository.Connection.Execute(sql, person);
        }

        protected void AddToCommandLog<T>(Guid entityId, string status, T data)
        {
            var commandLog = new AddToCommandLogCommand
            {
                EntityId = entityId,
                Status = status,
                CommandType = data.GetType().Name,
                CommandData = data
            };

            Mediator.Execute(commandLog);
        }
    }
}