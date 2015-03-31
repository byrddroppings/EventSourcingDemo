using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;

namespace MediatorLib
{
    public class QueryMediator : IQueryMediator
    {
        private readonly IContainer _container;

        public QueryMediator(IContainer container)
        {
            _container = container;
        }

        public T Query<T>(IQuery<T> request)
        {
            var defaultHandler = GetHandler(request);
            var result = defaultHandler.Handle(request);
            return result;
        }

        private QueryHandler<TResponse> GetHandler<TResponse>(IQuery<TResponse> request)
        {
            var handlerType = typeof (IQueryHandler<,>).MakeGenericType(request.GetType(), typeof (TResponse));
            var handler = GetInstance(handlerType);

            var wrapperType = typeof (QueryHandler<,>).MakeGenericType(request.GetType(), typeof (TResponse));
            var wrapper = Activator.CreateInstance(wrapperType, handler);

            return (QueryHandler<TResponse>) wrapper;
        }

        private object GetInstance(Type handlerType)
        {
            var handlers = _container.GetAllInstances(handlerType).Cast<object>().ToList();

            if (handlers.Count == 0)
                throw new InvalidOperationException("No Handler Found: " + handlerType);

            if (handlers.Count > 1)
                throw new InvalidOperationException("Multiple (" + handlers.Count + ") Handlers Found: " + handlerType);

            return handlers[0];
        }

        private abstract class QueryHandler<TResult>
        {
            public abstract TResult Handle(IQuery<TResult> message);
        }

        private class QueryHandler<TCommand, TResult> : QueryHandler<TResult> where TCommand : IQuery<TResult>
        {
            private readonly IQueryHandler<TCommand, TResult> _inner;

            public QueryHandler(IQueryHandler<TCommand, TResult> inner)
            {
                _inner = inner;
            }

            public override TResult Handle(IQuery<TResult> message)
            {
                return _inner.Handle((TCommand) message);
            }
        }
    }
}