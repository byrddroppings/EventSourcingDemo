namespace MediatorLib
{
    public class Mediator : IMediator
    {
        private readonly ICommandMediator _commandMediator;
        private readonly IQueryMediator _queryMediator;
        private readonly IValidateMediator _validateMediator;

        public Mediator(IQueryMediator queryMediator, ICommandMediator commandMediator,
            IValidateMediator validateMediator)
        {
            _queryMediator = queryMediator;
            _commandMediator = commandMediator;
            _validateMediator = validateMediator;
        }

        public T Query<T>(IQuery<T> query)
        {
            var validate = GetType().GetMethod("TryValidate").MakeGenericMethod(query.GetType());
            validate.Invoke(this, new object[] {query});
            return _queryMediator.Query(query);
        }

        public void Execute<T>(T command)
        {
            TryValidate(command);
            _commandMediator.Execute(command);
        }

        public void TryExecute<T>(T command)
        {
            TryValidate(command);
            _commandMediator.TryExecute(command);
        }

        public void Validate<T>(T item)
        {
            _validateMediator.Validate(item);
        }

        public void TryValidate<T>(T item)
        {
            _validateMediator.TryValidate(item);
        }
    }
}