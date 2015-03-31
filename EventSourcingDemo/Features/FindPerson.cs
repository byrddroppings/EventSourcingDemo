using System;
using System.Linq;
using MediatorLib;
using Dapper;
using EventSourcingDemo.Models;

namespace EventSourcingDemo.Features
{
    public class FindPersonQuery : IQuery<Person>
    {
        public Guid PersonId { get; set; }
    }

    public class FindPersonQueryHandler : IQueryHandler<FindPersonQuery, Person>
    {
        private readonly IRepository _repository;

        public FindPersonQueryHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Person Handle(FindPersonQuery request)
        {
            const string sql = @"select * from PersonView where PersonId = @PersonId";
            var person = _repository.Connection.Query<Person>(sql, request).FirstOrDefault();
            return person;
        }
    }
}