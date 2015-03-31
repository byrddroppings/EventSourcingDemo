using MediatorLib;
using Dapper;

namespace EventSourcingDemo.Features
{
    public class ClearDataCommand
    {
        public bool ClearLogs { get; set; }
        public bool ClearPeople { get; set; }
    }

    public class ClearDataCommandHandler : ICommandHandler<ClearDataCommand>
    {
        private readonly IRepository _repository;

        public ClearDataCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Execute(ClearDataCommand command)
        {
            if (command.ClearLogs)
                _repository.Connection.Execute("delete from CommandLog");

            if (command.ClearPeople)
                _repository.Connection.Execute("delete from Person");
        }
    }
}