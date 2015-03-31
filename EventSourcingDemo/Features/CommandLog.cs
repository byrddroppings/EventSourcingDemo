using System;
using System.Runtime.Serialization.Formatters;
using MediatorLib;
using Dapper;
using EventSourcingDemo.Models;
using Newtonsoft.Json;

namespace EventSourcingDemo.Features
{
    public class AddToCommandLogCommand
    {
        public Guid EntityId { get; set; }
        public string Status { get; set; }
        public string CommandType { get; set; }
        public object CommandData { get; set; }
    }

    public class AddToCommandLogCommandHandler : ICommandHandler<AddToCommandLogCommand>
    {
        private readonly IRepository _repository;

        public AddToCommandLogCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Execute(AddToCommandLogCommand command)
        {
            var commandLog = new CommandLog
            {
                EntityId = command.EntityId,
                Status = command.Status,
                Command = command.CommandType,
                Data = Serialize(command.CommandData)
            };

            const string sql = @"insert into CommandLog(EntityId,Command,Status,Data)
                                 values(@EntityId,@Command,@Status,@Data)";

            _repository.Connection.Execute(sql, commandLog);
        }

        private string Serialize(object instance)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Full
            };

            return JsonConvert.SerializeObject(instance, Formatting.Indented, serializerSettings);
        }
    }

}