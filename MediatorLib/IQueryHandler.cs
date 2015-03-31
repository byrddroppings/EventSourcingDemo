namespace MediatorLib
{
    public interface IQueryHandler<in TRequest, out TResponse>
    {
        TResponse Handle(TRequest request);
    }
}