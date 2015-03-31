using System;
using System.Linq;
using StructureMap;

namespace MediatorLib
{
    public class CommandMediator : ICommandMediator
    {
        private readonly IContainer _container;

        public CommandMediator(IContainer container)
        {
            _container = container;
        }

        public void Execute<T>(T command)
        {
            var handler = TryGetInstance<ICommandHandler<T>>();

            if (handler == null)
                throw new InvalidOperationException("No Handler Found: " + typeof(T));

            handler.Execute(command);
        }

        public void TryExecute<T>(T command)
        {
            var handler = TryGetInstance<ICommandHandler<T>>();

            if (handler == null)
                return;

            handler.Execute(command);            
        }

        private T TryGetInstance<T>()
        {
            var handlers = _container.GetAllInstances<T>().ToList();

            if (handlers.Count > 1)
                throw new InvalidOperationException("Multiple (" + handlers.Count + ") Handlers Found: " + typeof(T));

            return handlers.Count == 1 ? handlers[0] : default(T);
        }
    }
}