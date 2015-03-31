using System;
using System.Linq;
using MediatorLib;
using Dapper;
using EventSourcingDemo.Models;

namespace EventSourcingDemo.Features
{
    public class ViewModelQuery : IQuery<ViewModel>
    {
        public ViewModelQuery(Guid? personId)
        {
            PersonId = personId;
        }

        public Guid? PersonId { get; private set; }
    }

    public class ViewModelQueryHandler : IQueryHandler<ViewModelQuery, ViewModel>
    {
        private readonly IMediator _mediator;
        private readonly IRepository _repository;

        public ViewModelQueryHandler(IMediator mediator, IRepository repository)
        {
            _mediator = mediator;
            _repository = repository;
        }

        public ViewModel Handle(ViewModelQuery request)
        {
            var viewModel = new ViewModel();
            viewModel.People = GetAllPeople();

            if (request.PersonId.HasValue)
            {
                var personId = request.PersonId.Value;
                viewModel.Person = _mediator.Query(new FindPersonQuery { PersonId = personId });
                viewModel.Commands = GetCommandLog(personId);
                viewModel.PendingCommands = viewModel.Commands.Where(c => c.Status == "Pending").ToArray();
            }

            return viewModel;
        }

        private CommandLog[] GetCommandLog(Guid personId)
        {
            const string sql = @"select * from CommandLog where EntityId = @PersonId order by CommandLogId";
            var pendingCommands = _repository.Connection.Query<CommandLog>(sql, new {PersonId = personId});
            return pendingCommands.ToArray();
        }

        private Person[] GetAllPeople()
        {
            const string sql = @"select * from PersonView";
            var people = _repository.Connection.Query<Person>(sql).ToArray();
            return people;
        }
    }
}